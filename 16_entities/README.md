# Entities

Tulisan ini adalah rangkuman dari bab 16 buku Scott Millett, serta ditambahkan dengan beberapa informasi lainnya. Harap mahasiswa membaca lagi bab tersebut setelah membaca rangkuman ini karena banyak contoh code lain di dalam buku yang dapat membantu lebih memahami karakteristik dari entities. Pahami juga bab sebelumnya (bab 15) karena di dalam bab ini akan membahas hubungan entities dengan value object. Bila ada pertanyaan, langsung tanyakan di grup chat.

Seperti yang sudah dijelaskan di bab sebelumnya, perbedaan mendasar entity dan value object adalah entity memiliki ID, sedangkan value object tidak memiliki ID. Di dalam sebuah entity, terdapat atribut value objects yang merupakan bagian dari entity tersebut.

Untuk memutuskan suatu class dalam subdomain adalah entity atau bukan, bisa saja datang dari pengalaman dan keputusan developer masing-masing. Terutama pada class-class yang sudah nampak jelas entity atau value object. 

Namun langkah yang lebih tepat, keputusan bisa diambil dari hasil berbincang dengan domain expert. Karena entity atau bukan, tergantung dari problem domain yang dihadapi (context dependent).

Di bukunya, Scott Millett menggunakan contoh class: Money. Money bisa saja dianggap value object pada problem domain yang umum (misal: e-commerce).
Pada aplikasi yang umum ada, sistem tidak peduli apakah uang yang keluar dan masuk adalah uang yang sama. Sistem hanya peduli value-nya saja. 

Contoh: Amir berbelanja dengan melakukan setoran selembar uang 50.000 ke ATM, lalu mentransfer 50.000 kepada Budi. Setelah itu, Budi melakukan penarikan selembar uang 50.000 di ATM. Sistem kebanyakan tidak peduli apakah 50.000 yang ditarik Amir dan BUdi adalah lembar uang yang sama.

Pada problem domain lain, misalnya sistem untuk mendeteksi kegiatan money laundry. Kita perlu mencatat setiap nomor seri pada lembar uang untuk setiap transaksi yang terjadi. Pada problem domain money laundry, Money bersifat sebagai entity, bukan lagi value object.

Domain expert tentunya tidak semua paham istilah IT. Kita harus bisa menangkap sinyal implisit tanda-tanda bahwa class adalah sebuah entity.

Contoh yang diberikan Scott Millet:

Pada sebuah aplikasi booking hotel, domain expert memutuskan: "tidak boleh ada pengunjung yang memesan hotel yang berbeda pada tanggal yang sama, kalaupun hotelnya memiliki nama dan harga yang sama pun tidak boleh"

Dari pesan domain expert diatas, secara implisit domain expert bilang jangan sampai kita menggunakan attribute-based equality. Kita harus dapat membedakan hotel walaupun memiliki nama yang sama. Berarti hotel akan memiliki field ID.

Selain perkara ID, salah satu sinyal dari entity adalah domain expert menyatakan class memiliki state lifecycle. Contohnya, dalam obrolan domain expert menggunakan istilah: "order diterima", "pembayaran order dikonfirmasi", "order dikirim". Disini terlihat bahwa Order memiliki beberapa state.

## Implementing Entities

### Assigning Identifer

Berbeda dengan value object, entities dibandingkan berdasarkan IDnya. 

Pada contoh class Mahasiswa di bab sebelumnya, fungsi `Equals` cukup membandingkan NIM saja.

```csharp
public override bool Equals(object obj) {
    var m = obj as Mahasiswa;
    if (m == null) return false;

    return m.NIM.Equals(this.NIM);
}
```

Opsi yang dapat dipakai untuk ID dari entity adalah:
- Natural Keys: diambil dari field yang memang adalah bagian data dari class tersebut. Pastikan field ini unik dan hampir tidak mungkin di-update. Misalkan pada class Mahasiswa, ternyata NIM mahasiswa bisa diganti. Hal ini akan sangat repot dan beresiko. Kalau aplikasi hanya berjalan di satu database, bisa saja pakai fitur constraint UPDATE CASCADE. Namun, kalau aplikasi sudah berjalan di banyak database, kita harus mengurus pergantian NIM di banyak tempat. Belum lagi jika ada user yang sudah men-download data dalam bentuk excel. Pergantian ID dalam excel juga harus diinfokan ke user.
- Auto-increment integer: solusi yang tentunya pernah kalian pakai di semester sebelumnya. Kita membuat field baru yang akan dimulai dari angka satu dan terus bertambah mengikuti data-data baru yang kita masukkan.
- Custom String: format string yang dibuat sendiri. Biasanya format ini berhubungan dengan kebutuhan bisnis agar lebih mudah dipahami oleh user. Contohnya, di BINUS kita menggunakan kode matakuliah seperti COMP6114. Disini COMP berarti computer science dan 6114 adalah angka unik yang ditentukan oleh BINUS. Contoh lain, 16 digit nomor KTP Indonesia menggunakan terdiri dari 6 digit kode wilayah + 6 digit tanggal lahir + 4 digit auto-increment.
- GUID/UUID: 128 bit alias 32 karakter hexadecimal yang di-generate random. Sejauh ini, terdapat [5 versi cara random UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier). GUID adalah istilah Microsoft untuk UUID. UUID dipastikan unik tanpa perlu ada pengecekan, kemungkinan terjadi collision sangat-sangat kecil. 

Trivia: Bila Anda penasaran angka eksak kemungkinan terjadinya collision pada UUID, coba tonton dan pahami [Birthday Paradox](https://www.youtube.com/watch?v=ofTb57aZHZs).

### Pushing Behavior into Value Objects and Domain Services

Agar menjaga tidak terjadi [Bloaters](https://sourcemaking.com/refactoring/smells/bloaters) dan tidak melanggar SRP, entity dijaga agar hanya bertanggungjawab seputaran identity dan state lifecyle, semua behavior lain diusahakan diletakkan pada value object atau domain service. Domain service akan dibahas setelah UTS.

Perhatikan contoh code di Listing 16-6 halaman 369 buku Scott Millett. Terdapat class HolidayBooking yang memiliki value object Stay. Dari ketentuan bisnis, pengunjung minimal booking 3 hari, dan divalidasi bahwa tanggal mulai menginap (check-in) harus lebih awal dibanding tanggal akhir menginap (check-out). Nah, ketentuan bisnis ini kita letakkan di dalam value object Stay, jangan dibiarkan di dalam Entity.

```csharp
public class Stay {
    public Stay(DateTime firstNight, DateTime lastNight) {
        if (firstNight > lastNight)
        throw new FirstNightOfStayCannotBeAfterLastNight();
        if (DoesNotMeetMinimumStayDuration(firstNight, LastNight))
        throw new StayDoesNotMeetMinimumDuration();
        this.FirstNight = firstNight;
        this.LastNight = lastNight;
    }
    ...
}
```

### Validating and Enforcing Invariants

Sama seperti value object, entities harus dijaga agar selalu dalam keadaan valid.

Sedikit tambahan, entities dapat menjaga invariant tergantung dari state saat ini.

Misalkan pada contoh code di Listing 16-7 halaman 372 buku Scott Millett, terdapat validasi method `Reschedule()` yang boleh dilakukan bila booking belum dikonfirmasi. Bila booking sudah dikonfirmasi, maka jadwal tidak boleh diganti.

```csharp
public class FlightBooking {
    private bool confirmed = false;
    public void Reschedule(DateTime newDeparture) {
        if (confirmed) throw new RescheduleRejected();
        this.DepartureDate = newDeparture;
    }

    public void Confirm() {
        this.confirmed = true;
    }
}
```

### Focusing on Behavior, Not Data

Jangan sampai mengeksploitasi setter-getter. Kita harus menjaga class mengikuti prinsip [Tell, Don't Ask](https://martinfowler.com/bliki/TellDontAsk.html).

Perhatikan contoh code `Confirm()` diatas. class FlightBooking tidak membuka data `confirmed` lewat setter-getter. class FlightBooking hanya mengizinkan class lain untuk menyuruh melakukan perubahan terhadap data `confirmed` menggunakan method `Confirm()`.

### Avoiding the “Model the Real‐World” Fallacy

Pada pembukaan part 1 di buku Eric Evans, terdapat analogi peta dunia. Eric Evans mengingatkan bahwa untuk berjalan dengan baik, model dari sistem tidak perlu sama persis dengan hal yang terjadi di dunia nyata. Dalam analoginya, untuk kebutuhan navigasi, manusia tidak pernah menggambar bumi sama persis, dengan bentuk dan skala yang sama persis. Manusia malah menggambar bumi di bidang datar, dalam bentuk atlas.

Terdapat contoh class Money pada awal rangkuman ini. Tentunya tidak semua aplikasi butuh mencatat serial number dari masing-masing lembar uang. Umumnya, aplikasi cukup peduli dengan value dan currency-nya saja.

### Designing for Distribution

Pada aplikasi monolithic, aman-aman saja bila semua value object dicampur dalam satu class. Namun, bila aplikasi dijalankan di distributed system / microservice, masalah terjadi. Bila data tersebar di banyak sistem / database, akan sangat memakan performa untuk melakukan join.

Lihatlah contoh code di Listing 16-11 dan 16-12 halaman 378 buku Scott Millett. Disitu logika Customer dipecah dalam beberapa bounded context. Di masing-masing class di bounded context, class memiliki reference ID ke entity-nya. Sehingga logika dapat dijalankan terpisah, namun bila sewaktu-waktu data benar-benar butuh digabungkan, kita bisa menggabungkannya lewat reference ID ini.

## Common Entity Modelling Principles and Patterns

### Implementing Validation and Invariants with Specifications

**Specifications** adalah satu buah class dengan code yang hanya berisi satu fungsi validasi. Lihat contoh di buku halaman 380.

**Specifications** Ini adalah alternatif untuk melakukan validasi. Tidak semua tim setuju dengan hal ini karena ada kecenderungan [lazy class](https://sourcemaking.com/refactoring/smells/lazy-class). Keuntungannya adalah penamaan lebih jelas dan dapat lebih mudah dibuat unit test-nya. **Specifications** cocok digunakan bila code dikembangkan lebih lanjut menggunakan design pattern strategy dan/atau composite.

### Avoid the State Pattern; Use Explicit Modeling

Dalam state pattern, state akan disimpan dalam bentuk atribut di dalam class entity. Bisa dilihat di listing 16-18 halaman 382.

Namun, bila menggunakan state pattern, dikhawatirkan [Refused Bequest](https://sourcemaking.com/refactoring/smells/refused-bequest) karena bisa saja bila abstract class menyedikan 4 method, ternyata setelah di-extends, class anak hanya memakai 2 method, 2 method lagi dibiarkan kosong.

Oleh karena itu, Scott Millett menawarkan alternatif lain. State dibuat menjadi class secara eksplisit. Perhatikan listing 16-19 di halaman 384.

### Avoiding Getters and Setters with the Memento Pattern

Memento pattern akan dijelaskan lebih detail di semester depan pada matakuliah Framework Layer Architecture.

Singkatnya, di dalam memento pattern, class memiliki method `snapshot`. Method ini berfungsi untuk meng-copy data-data di dalam class ke dalam object yang baru. Object baru bisa saja berupa class lain yang memiliki struktur data yang mirip. Hal ini bertujuan untuk melindungi data di object yang asli. 

Analogikan method ini seperti kita memfoto sebuah benda. Kita dapat melihat benda tersebut lewat foto. Kita bisa saja coret-coret di foto tersebut. Namun, apapun yang kita lakukan terhadap foto ini, tidak akan mengubah kondisi benda yang asli.

Snapshot bisa disimpan ke dalam array yang diurus oleh class yang diistilahkan sebagai `CareTaker`. Kita bisa menggunakan `CareTaker` untuk menyimpan history snapshot dan melakukan mekanisme undo-redo dengan mudah.

Bila Anda melihat contoh code di buku Scott Millett, Memento yang digunakan tidak memakai `CareTaker`. Scott Millett hanya meminjam konsep `snapshot` untuk menyelamatkan objek asli dari bahaya setter-getter.

### Favor Hidden‐Side‐Effect‐Free Functions

Sebenarnya ini merupakan konsep clean code yang umum, tidak harus hanya diimplementasi di entity.

Jangan sampai fungsi yang kita buat mengandung hidden-side-effect. Pastikan nama fungsi mencerminkan apa yang dilakukan di dalamnya.

```csharp
public class Foo {
  private int value;
  public int GetValue() {
    value++;
    return value;
  }
}
```

Pada code diatas, terdapat method `GetValue()`. Programmer yang memanggil method ini tentunya berekspektasi bahwa yang dilakukan class Foo hanya mengembalikan value. Ternyata di dalam method, value juga ditambah satu.