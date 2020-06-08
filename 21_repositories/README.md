# Repositories

Tulisan ini adalah rangkuman dari bab 21 buku Scott Millett, serta ditambahkan dengan beberapa informasi lainnya. Harap mahasiswa membaca lagi bab tersebut setelah membaca rangkuman ini karena banyak contoh code lain di dalam buku yang dapat membantu lebih memahami Repositories.

---

Code sebelum ditambahkan repository: https://github.com/mrp130/psd/tree/4-factory

Code setelah ditambahkan repository: https://github.com/mrp130/psd/tree/5-repository

---

## Definisi

Repository adalah building block yang bertugas untuk mengurus masalah *persistence* (menyimpan data, misal: ke database) dan *hydrate* (mengambil data, misal: dari database). Hal ini bertujuan untuk memisahkan hubungan antara **domain model** dan **data model**. Seperti yang sering kita bahas sebelumnya, **domain model** harus dijaga se-POCO mungkin tanpa memperdulikan urusan infrastruktur/teknologi lain.

Repository juga menjaga agar antar **domain object** hanya mengakses **aggregate** lain melalui **aggregate root**-nya. Oleh karena itu, praktik yang umum dilakukan adalah, kita akan membuat satu buah repository untuk masing-masing **aggregate**.

Biasanya yang menggunakan repository adalah application layer dari project Anda. Bila dibutuhkan, repository bisa diberikan kepada **domain service** atau building block lain via **dependency injection** atau **service locator**.

## Tujuan

Scott Millett menegaskan, tujuan utama dari repository bukanlah perkara code lebih mudah di-test. Bukan juga perkara kemudahannya bila sewaktu-waktu implementasi database perlu diganti. Misalkan awalnya menggunakan MySql kemudian pindah ke SqlServer. Kedua hal itu memang bisa dicapai dengan menggunakan repository, namun tujuan utama dari repository adalah: segregasi antara domain model dengan data model. 

Domain model harus bisa berkembang sendiri tanpa memperdulikan cara persistensinya ke database. Jangan sampai desain ERD mempengaruhi desain class diagram Anda. Kesalahan umum yang biasanya terjadi bila menerapkan OOP + Relational DB adalah: ERD dan UML selalu sama persis. Memang pada beberapa kasus project yang fiturnya masih minim, ERD dan bagian model (MVC) pada class diagram bisa saja memiliki atribut yang sama, namun seiring berkembangnya fitur, seharusnya dua hal ini bisa berkembang dengan sendirinya.

## Generic Repository

Salah satu cara menerapkan repository adalah menggunakan Generic Repository. Berikut contoh yang diberikan Scott Millett di halaman 483:

```cs
namespace DomainModel
{
  public interface IRepository<TAggregate, TId>
  {
    IEnumerable<TAggregate> FindAllMatching(Expression<Func<T, bool>> query);
    IEnumerable<TAggregate> FindAllMatching(string query);
    TAggregate FindBy(TId id);
    void Add(TAggregate aggregate);
    void Remove(TAggregate aggregate);
  }
}
```

Pada interface tersebut, repository akan mengikuti tipe yang dberikan (`TAggregate` dan `TId`). Idenya, Anda cukup melakukan satu kali code untuk mengurus semua repository yang Anda butuhkan. Hal ini sebenarnya bagus karena menghindari smell duplicate code.

Scott Millett menyatakan, sebaiknya penggunakan generic repository pada client class dihindari. Karena repository idealnya merupakan *explicit contract* yang bergantung pada **aggregate** masing-masing. Misalkan suatu aggregate butuh filter berdasarkan status = active. Berarti kita akan membuatkan sebuah method misalkan bernama *GetAllActive*. Sedangkan pada generic repository diatas, programmer memasukkan sendiri filter status = active ke dalam fungsi *FindAllMatching*. Hal ini berbahaya karena programmer bisa sengaja/tidak sengaja melakukan inject query yang tidak diinginkan. Selain itu, programmer juga kerepotan dalam melakukan code karena harus mengurus semua raw query di domain model.

Generic repository tetap bisa dimanfaatkan sebagai helper. Dibungkus di dalam Repository yang sudah berisi *explicit contract*. Contoh:

```cs
namespace DomainModel
{
  public interface ICustomerRepository
  {
    Customer FindBy(Guid id);
    IEnumerable<Customer> FindAllThatAreDeactivated();
    void Add(Customer customer);
  }
}
```

```cs
namespace Infrastructure.Persistence
{
  public class CustomerRepository : ICustomerRepository
  {
    private IRepository<Customer, Guid> _customersRepository;
    
    public Customers(IRepository<Customer, Guid> customersRepository) {}
    
    public IEnumerable<Customer> FindAllThatAreDeactivated()
    {
      _customersRepository.FindAllMatching(new CustomerDeactivatedSpecification()); 
    }
    
    public void Add(Customer customer)
    {
      _customersRepository.Add(customer);
    }
  }

}
```

Pada contoh diatas, `_customerRepository` merupakan generic repository bertipe `<Customer, Guid>`. Generic repository ini dimanfaatkan untuk mempermudah query pada method-method di dalam CustomerRepository. Perhatikan method `FindAllThatAreDeactivated` yang mendelegasikan pekerjaannya ke generic repository ke `FindAllMatching`.

## Strategi Persistensi

Untuk melakukan persistensi ke database, cara yang paling mudah adalah menggunakan ORM. Di ORM, model Anda akan di-map langsung ke table Anda di relational database.

Namun biasanya pada kasus tertentu, ORM memiliki keterbatasan. Misalnya ketika attribute pada class Anda adalah attribute private dan tidak ada setter dan/atau getter-nya. Bila terjadi hal seperti ini, ada beberapa kompromi yang harus dilakukan. Tentunya kompromi ini bisa menyebabkan enkapsulasi Anda bocor sedikit/banyak.

### Cara Kompromi

#### Public Setter / Getter

Getter dan Setter dibuat public. Mengorbankan enkapsulasi demi repository bisa melakukan persistence/hydrate data dengan mudah. Masalah yang bisa timbul contohnya: data dapat diubah tidak sesuai dengan invariant karena setter-nya dibuka. Lakukan kompromi ini bila Anda yakin public setter tidak terlalu masalah. Pastikan code review terus dilakukan di dalam tim Anda untuk menghindari kesalahan yang sengaja/tidak sengaja.

#### Memento

Bila enkapsulasi domain object tetap mau dijaga. Cara lain yang agak merepotkan adalah membuat class Memento-nya. Dengan menggunakan Memento, class utama akan membantu men-generate object Memento dengan data-datanya. public setter dan getter akan diletakkan di Memento, tidak di dalam class utama. Persistence dan hydrate pun dilakukan menggunakan class Memento ini.

Contoh implementasi Memento dapat dilihat pada [TicTacToeMemento](https://github.com/mrp130/psd/blob/master/Xyz/Game/RoomAggregate/TicTacToe.cs#L5) di code project XYZ. Memento tersebut bisa di-[generate](https://github.com/mrp130/psd/blob/master/Xyz/Game/RoomAggregate/TicTacToe.cs#L194) dan bisa di-[load](https://github.com/mrp130/psd/blob/master/Xyz/Game/RoomAggregate/TicTacToe.cs#L199) ke dalam object game TicTacToe.

```cs
public override object GetMemento()
{
  return new TicTacToeMemento(_p1, _p2, _currentPlayer, _size, _gameEnded, _currentSymbol, _board);
}

public override void LoadMemento(object memento)
{
  var m = memento as TicTacToeMemento;
  if (m == null) throw new Exception("wrong memento");

  this._p1 = m.P1;
  this._p2 = m.P2;
  this._currentPlayer = m.CurrentPlayer;
  this._currentSymbol = m.CurrentSymbol;
  this._gameEnded = m.GameEnded;
  this._board = m.Board;
  this._size = m.Size;
}
```

#### Event Stream

Menggunakan teknik event sourcing, kita menyimpan semua event yang terjadi dari awal object dibuat. Kemudian ketika proses hydrate, object akan dibuat ulang berdasarkan semua event yang lalu. Hal ini akan dibahas lebih lanjut di bab berikutnya mengenai event sourcing.

## Transaction Management dan Unit of Work

Ketika melakukan persistensi, idealnya kita perlu mengurus transaction pada database. Seperti yang Anda pelajari di kelas Database, biasanya database dilengkapi dengan query `BEGIN TRAN`, `COMMIT`, dan `ROLLBACK`.

Misal, suatu application layer menjalankan method `A` yang menjalankan query untuk mengurangkan uang dari saldo user. Kemudian dilanjutkan method `B` yang menjalankan query untuk mengurangkan stok barang di dalam gudang. Ternyata karena suatu hal, method `B` ini error. Tentunya kita perlu melakukan rollback agar query dari method `A` tidak benar-benar terjadi. Sehingga saldo user tidak jadi berkurang.

Untuk melakukan proses commit/rollback dengan lebih rapi, biasanya developer menerapkan pattern Unit of Work yang telah dideskripsikan Martin Fowler ([baca disini](https://martinfowler.com/eaaCatalog/unitOfWork.html)). Contoh pada code project PT. XYZ dapat dilihat [disini](https://github.com/mrp130/psd/blob/master/Xyz/Game/database/postgres/PostgresUnitOfWork.cs). Penerapannya dapat dilihat di unit test [UnitOfWorkTest.cs](https://github.com/mrp130/psd/blob/master/Xyz/GameTest/UnitOfWorkTest.cs).

Bila Anda menggunakan .NET Entity Framework, sudah disediakan class **DbContext** yang mengurus masalah transaksi ini.

## Peran Lain Repository

### Entity ID Generation

Men-generate ID dari entity. Hal ini bisa diletakkan ke sistem database langsung, atau ada juga yang perlu dilakukan menggunakan logika tertentu di code repository.

### Collection Summaries

Repository mengurus summaries seperti `sum`, `average`, `count`, dan summary lainnya. Hal ini sebisa mungkin dilakukan sedekat mungkin dengan raw data karena memiliki performa yang lebih baik. Misal ketika ingin melakukan `sum` dari banyak data, alih-alih kita tarik semuanya dan melakukan `sum` menggunakan for, sebaiknya kita langsung menggunakan query `select SUM(...) ...` saja langsung di database.

### Concurrency

Repository mengurus masalah pessimistic concurrency. Di dalam repository, bisa ditambahkan code untuk menentukan metode locking.

### Audit Trails

Repository mengurus semua pencatatan metadata untuk keperluan audit. Misal, setiap ada perubahan, maka data lama dan baru dicatat dalam table history. Atau contoh lain yang lebih mudah: setiap ada perubahan data, maka kolom `updated_at` diganti menjadi timestamp sekarang.

## Anti Pattern

Berikut adalah kesalahan penerapan repository menurut Scott Millett.

### Anti Pattern: Ad Hoc Queries

Yang dimaksud dengan ad-hoc query adalah, sebuah code yang memungkinkan kita untuk menyelipkan raw query. Misalnya lewat passing parameter.

```cs
public interface ICustomerRepository
{
  void Add(Customer customer);
  Customer FindBy(Guid Id);
  IEnumerable<Customer> FindBy(CustomerQuery query);
}
```

Seperti statemen yang sudah dibahas sebelumnya, repository harus dibuat seekplisit mungkin. Bila developer sampai membiarkan adanya ad-hoc query, berarti interface kurang eksplisit.

### Anti Pattern: Lazy Loading

Lazy loading sebenarnya bagus untuk dilakukan. Ketika kita punya data yang banyak, misalnya satu juta data, tidak mungkin kita langsung load semuanya dari database (eager loading). Pasti developer butuh lazy loading untuk melakukan pagination.

Masalahnya, di dalam aggregate, berbahaya dilakukan lazy loading karena bisa merusak pengecekan invariant. Disarankan membuat interface dengan filter yang lebih eksplisit dan menggunakan teknik *collection summaries* pada repository untuk menekan performa.

Pagination menggunakan lazy loading pastinya merupakan fitur yang sering dibutuhkan terutama dari sisi frontend. Untuk mengakalinya, Anda bisa menerapkan cara berikut [ini](https://medium.com/@stevesun21/pagination-in-domain-driven-design-c038c6858ac0).


### Anti Pattern: Reporting

Jangan gunakan repository untuk keperluan reporting.

Idealnya, layaknya aggregate, repository dibuat berdasarkan kebutuhan domain. Repository tidak dibuat mengikuti UI maupun reporting.

Reporting sebaiknya diambil menggunakan teknik lain. Misalnya menggunakan OLAP.

---

## Implementasi

Untuk mengimplementasikan repository, pilihlah terlebih dahulu tempat dimana data Anda akan disimpan. Cara melakukan persistensi / pengambilan data juga beragam. Anda bisa melakukan raw query langsung ke database via driver yang disediakan. Atau bila menggunakan .NET Framework, bisa menggunakan Entity Framework yang telah disediakan .NET seperti yang telah diajarkan di kelas praktikum.

Dalam contoh ini, misalkan data akan disimpan ke dalam database `Postgresql`. Agar lebih terbayang proses query-nya, implementasi pada contoh kasus ini akan menggunakan raw query.

### Update Database

Agar dapat melakukan penyimpanan ke `postgres`, ada konfigurasi project yang perlu ditambahkan:

1. Pastikan `TargetFramework` memiliki versi minimal **2.0** (`netstandard2.0`) pada file [Game.csproj](../Xyz/Game/Game.csproj)

```xml
<PropertyGroup>
  <TargetFramework>netstandard2.0</TargetFramework>
</PropertyGroup>
```

2. Add package **Npgsql** dengan cara menjalankan command berikut. Pastikan Anda menjalankan command di dalam folder Game.

```
dotnet add package Npgsql
```

Pastikan tidak ada error. Sekarang seharusnya file [Game.csproj](../Xyz/Game/Game.csproj) sudah memiliki reference ke **Npgsql**.

3. Karena nantinya ada beberapa data yang disimpan dalam format `json`, kita membutuhkan library untuk melakukan serialize/deserialize json. Add package **System.Text.Json** dengan cara menjalankan command berikut di dalam folder Game.

```
dotnet add package System.Text.Json
```

### Code

Code PT.XYZ yang sudah ditambahkan repository dapat dilihat di: https://github.com/mrp130/psd/tree/5-repository.

Perhatikan semua `interface` dari repo dan unit of work dletakkan di `namespace` domain model. Sedangkan implementasinya, dibuatkan `namespace` terpisah bernama `postgres` [disini](https://github.com/mrp130/psd/tree/master/Xyz/Game/database/postgres).
