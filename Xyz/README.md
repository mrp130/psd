# XYZ Gaming

Folder ini berisikan C# solution untuk PT.XYZ yang ingin membuat project *casual gaming*.

Project ini digunakan untuk keperluan studi kasus matakuliah COMP6114 - Pattern Software Design. Code akan berfokus pada pengembangan di sisi *backend*.

Setiap pertemuannya, akan ditambahkan code-code baru sesuai dengan topik di pertemuan tersebut.

## Fitur

Fitur-fitur dibawah ini bisa bertambah sesuai kebutuhan studi kasus.

- `User` memiliki `experience` (Exp).
- `Exp` bertambah setiap kali `User` menyelesaikan sebuah `game`. Menang-kalah mempengaruhi jumlah `exp` yang diperoleh.
- PT. XYZ ingin membuat berbagai macam *game* kasual yang dapat dipilih ketika `user` membuat `room`, seperti: Tic-Tac-Toe, Rock-Paper-Scissor, Hangman, Big 2, dan sebagainya.
- `Exp` yang diperoleh bisa sementara dimodifikasi mengikuti event yang diselenggarakan oleh PT. XYZ. Misalnya, event double exp untuk game Tic-Tac-Toe selama bulan Mei.

### Game Detail

#### Tic-Tac-Toe

Menang: +5 exp

Kalah: +2 exp
