# Event Sourcing

Tulisan ini adalah rangkuman dari bab 22 buku Scott Millett, serta ditambahkan dengan beberapa informasi lainnya. Harap mahasiswa membaca lagi bab tersebut setelah membaca rangkuman ini karena banyak contoh code lain di dalam buku yang dapat membantu lebih memahami Event Sourcing.

---

Code sebelum ditambahkan event sourcing: https://github.com/mrp130/psd/tree/5-repository

Code setelah ditambahkan event sourcing: https://github.com/mrp130/psd/tree/6-event-sourcing

---

## Definisi

Event sourcing adalah teknik untuk menyimpan dan mengakses data berdasarkan event stream. Di dalam event stream, data disimpan dalam bentuk historik.

Contoh:

Cara menyimpan saldo client tanpa event stream:

| ID                                   | Nama | Saldo   |
|--------------------------------------|------|---------|
| 951b3660-d8f5-488d-9632-d66691ad49b4 | Amir | 1000000 |
| 8c78f3d4-fb04-46c1-b996-cd1ff34b7f83 | Budi | 1500000 |

Pada tabel diatas, masing-masing user (Amir dan Budi) langsung disimpan saldonya. 

Misalkan terjadi transaksi. Budi mengirim uang sebesar 200.000 ke Amir. Maka tabel akan langsung di-update (misalnya menggunakan query db) menjadi:

| ID                                   | Nama | Saldo   |
|--------------------------------------|------|---------|
| 951b3660-d8f5-488d-9632-d66691ad49b4 | Amir | 1200000 |
| 8c78f3d4-fb04-46c1-b996-cd1ff34b7f83 | Budi | 1300000 |

Kolom saldo pun langsung di-update. Tidak ada history kapan Amir dan Budi melakukan transaksi. Kalaupun ada, biasanya hanya berupa table history. Bila data di kolom saldo dan table history tidak cocok, biasanya kita akan lebih percaya kolom saldo. Disini kolom saldo berperan sebagai *source of truth*.

---

Di event sourcing, *source of truth* dibalik. Kita akan menggunakan event sebagai *source of truth*. Snapshot seperti kolom saldo tetap boleh dihitung dan disimpan, namun hanya berlaku sebagai cache untuk meningkatkan performa komputasi. Bila terjadi ketidakcocokan antara snapshot dan event stream, kita akan mempercayai data di event stream.

Jadi, bila menggunakan event sourcing, bentuk tabelnya akan:

table user:

| ID                                   | Nama |
|--------------------------------------|------|
| 951b3660-d8f5-488d-9632-d66691ad49b4 | Amir |
| 8c78f3d4-fb04-46c1-b996-cd1ff34b7f83 | Budi |

table transaksi:

| ID | User ID                              | Amount  | Notes                                                 | Created At |
|----|--------------------------------------|---------|-------------------------------------------------------|------------|
| 1  | 951b3660-d8f5-488d-9632-d66691ad49b4 | 1000000 | Deposit Awal                                          | ...        |
| 2  | 8c78f3d4-fb04-46c1-b996-cd1ff34b7f83 | 1500000 | Deposit Awal                                          | ...        |
| 3  | 8c78f3d4-fb04-46c1-b996-cd1ff34b7f83 | -200000 | kirim dana ke 951b3660-d8f5-488d-9632-d66691ad49b4    | ...        |
| 4  | 951b3660-d8f5-488d-9632-d66691ad49b4 | +200000 | terima dana dari 8c78f3d4-fb04-46c1-b996-cd1ff34b7f83 | ...        |

Perhatikan table transaksi. Setiap ada transaksi baru, maka data akan di-**append** (ditambahkan) sebagai row baru. Sifat data di dalam event stream memanglah harus **immutable**. Tidak ada row yang boleh di-update isinya setelah di-insert. Karena sifat **immutable** ini, developer lebih nyaman menjadikan event stream sebagai *single source of truth*.

Lalu bagaimana cara melihat saldo dari Amir atau Budi? Pada kasus ini, kita tinggal melakukan `SUM` pada database.

```sql
-- melihat saldo Amir
SELECT SUM(amount) FROM transaction WHERE user_id = '951b3660-d8f5-488d-9632-d66691ad49b4';
```

Tidak semua event sourcing bisa diselesaikan menggunakan query agregasi di database. Kadangkala, kita perlu menjalankannya via code. Misal, Anda membuat game catur yang menyimpan semua gerakannya sebagai event stream. Untuk mendapatkan state saat ini, Anda perlu menjalankan semua gerakan dari awal (saat semua bidak masih di posisi awal) sampai gerakan terakhir. Proses ini cocok diletakkan di building block Factory.

## Kegunaan Event Sourcing

### Temporal Queries

Kita bisa memilih ingin melihat data di waktu kapan. Misalkan pada contoh saldo diatas. Kita bisa melihat saldo Amir pada akhir bulan Desember 2019 dengan mudah.

```sql
-- melihat saldo Amir di akhir bulan Desember 2019
SELECT SUM(amount) FROM transaction WHERE user_id = '951b3660-d8f5-488d-9632-d66691ad49b4' AND created_at <= '2019-12-31T23:59:59';
```

### Projections

Dengan event sourcing, kita juga dapat dengan mudah menggabungkan beberapa event stream dengan data serupa. Setelah digabungkan, data ini bahkan bisa kita broadcast lagi menjadi event yang baru ke dalam *message broker*. Misalnya, di buku Scott Millett, beliau mengambil semua transaksi user yang berumur 16-22 tahun.

### Snapshots

Ini merupalan bagian yang cukup penting dan perlu Anda buat ketika mengimplementasi event sourcing. Ketika data masih sedikit, tentunya query agregat pada database masih aman-aman saja. Query masih berjalan dengan cepat. Namun seiring bertambahnya data menjadi jutaan. Tidak mungkin Anda terus-menerus melakukan query untuk jutaan data tersebut setiap kali ingin melihat saldo seorang user.

Untuk itu, Anda perlu membuat snapshot di titik tertentu pada event stream. Pembuatan snapshot bisa dipicu dengan metode yang bermacam-macam tergantung developer-nya. Contohnya, snapshot bisa dibuat tiap jam 12 malam menggunakan scheduler. Atau contoh lain yang lebih mudah dan populer adalah, snapshot dibuat tiap kali data mencapai kelipatan tertentu.

Misalnya di code PT. XYZ, exp dibuat menjadi event sourcing dan snapshot dibuat tiap kali jumlah transaksi exp mencapai kelipatan 100 (modulus 100) pada user tertentu. Silahkan lihat detail code [disini](https://github.com/mrp130/psd/blob/master/Xyz/Game/database/postgres/PostgresUserRepository.cs#L68). Unit test-nya bisa dilihat [disini](https://github.com/mrp130/psd/blob/master/Xyz/GameTest/UserRepoTest.cs#L64).

Misalnya Amir mendapat exp sejumlah 220 kali. Bila developer memutuskan menggunakan kelipatan 100, maka akan dibuat snapshot dua kali. Di titik transaksi ke-100 dan juga di titik transaksi ke 200. Kemudian ketika melakukan query agregat, kita hanya perlu `sum` 20 data saja (data ke 201 sampai ke 220). Hasil agregat ini tinggal ditambahkan dengan snapshot paling akhir (snapshot titik 200). 

--- 

Struktur table `exp_snapshot` dapat dilihat di [create_table.sql line 58](https://github.com/mrp130/psd/blob/master/Xyz/Game/database/postgres/create_table.sql#L58).