# Value Objects

Tulisan ini adalah rangkuman dari bab 15 buku Scott Millett, serta ditambahkan dengan beberapa informasi lainnya. Harap mahasiswa membaca lagi bab tersebut setelah membaca rangkuman ini karena banyak contoh code lain di dalam buku yang dapat membantu lebih memahami karakteristik value object.

Dua hal paling mendasar dari building blocks DDD adalah `value object` dan `entity`. `Value object` dan `entity` memiliki karakteristik detail masing-masing. Namun, agar Anda lebih mudah membayangkan, karakteristik utama dari `value object` adalah tidak memiliki ID, sedangkan `entity` memiliki ID.

Contoh simpelnya seperti ini: terdapat obyek `Mahasiswa` yang memiliki `NIM`, `nama`, dan `tanggal lahir`.

`Mahasiswa` adalah sebuah `entity`. Object ini dapat  kita kenali dari IDnya. `NIM` bisa dipertimbangkan sebagai primary key karena merupakan [natural key](https://en.wikipedia.org/wiki/Natural_key) dari obyek `Mahasiswa`.

Sedangkan `NIM`, `nama`, dan `tanggal lahir`, adalah state dari entity `Mahasiswa`. Ketiga atribut ini bisa dipertimbangkan dibuat class `value object`-nya atau dibiarkan primitif menggunakan tipe data/class bawaan bahasa pemrograman seperti string dan date. Pertimbangan paling mudahnya adalah cek apakah akan menghadirkan smell [Primitive Obsession](https://sourcemaking.com/refactoring/smells/primitive-obsession) atau tidak.

Misalkan `NIM` dipertimbangkan harus dibuat class-nya sendiri karena ternyata `NIM` punya banyak behavior (value object harus behavior-rich, dibahas di bawah). Misalkan salah satu contoh behavior-nya: NIM di BINUS harus 10 digit.

Contoh class [Mahasiswa dan NIM](sample/Mahasiswa.cs):

```csharp
public class Mahasiswa
{
    private NIM NIM;
    private string name;
    private DateTime tanggalLahir;

    public Mahasiswa(string NIM, string name, DateTime tanggalLahir)
    {
        this.NIM = new NIM(NIM);
        this.name = name;
        this.tanggalLahir = tanggalLahir;
    }
}

public class NIM
{
    private string value;

    public NIM(string value)
    {
        if (value.Length != 10)
            throw new Exception("nim must be 10 char");

        this.value = value;
    }
}
```

Contoh lain, terdapat obyek `Produk` yang memiliki atribut `nama` dan `harga`. 

Disini `produk` tidak memiliki natural key, namun `produk` perlu kita buat sebagai `entity`. Karena bila kebetulan ada produk lain yang memiliki `harga` dan `nama` yang sama, kedua produk tersebut tetap harus dapat dibedakan. 

Bila entity tidak punya natural key, biasanya developer akan membuat [surrogate key](https://en.wikipedia.org/wiki/Surrogate_key) dengan menggunakan auto-increment integer atau UUID/GUID.

`Harga` bisa dibiarkan primitif menggunakan float, atau bila `harga` memiliki behavior, kita bisa membuat class `value-object`-nya. Di buku Scott Millett, dicontohkan `harga` dibuat dari class `money` yang memiliki atribut `value` dan `currency`.

Class [Product](sample/Product.cs):
```csharp
public class Product
{
    public Guid Id { get; }
    private string name { get; set; }
    private Money price { get; set; }

    public Product(string name, Money price)
    {
        this.Id = Guid.NewGuid();
        this.name = name;
        this.price = price;
    }
}
```

Pada constructor class **Product**, guid dibuat random sebagai identitas dari obyek tersebut.

Class [Money](sample/Money.cs):
```csharp
public class Money
{
    private string currency;
    private float value;

    public Money(string currency, float value) 
    {
        if (currency.Length != 3)
            throw new Exception("currency must 3 char");

        this.currency = currency;
        this.value = value;
    }
}
```

Sebagai contoh, validasi class **Money** dibuat simpel. Hanya cek harus 3 karakter. Pada kenyataannya, validasi **Money** bisa diperlengkap, misal terdapat daftar mata uang yang benar-benar berlaku sekarang. Behavior-nya pun bisa ditambah misalkan ada method untuk konversi ke mata uang lain. Bahkan currency bisa [diangkat](https://sourcemaking.com/refactoring/replace-data-value-with-object) menjadi class sendiri jika ada kecenderungan primitive obsession.

## Karakteristik

### Identity-less

Value object tidak boleh punya ID. Biasanya value object akan menempel ke entity tertentu agar memiliki makna dan dapat digunakan dalam sistem. Value object berperan sebagai state dari entity tersebut.

Mungkin akan muncul pertanyaan, berarti bila value object disimpan di dalam table ERD di database, dan kebetulan value object disimpan terpisah table dengan entity-nya, value object tidak boleh memiliki ID/primary key di dalam table?

Untuk kasus persistensi (penyimpanan ke DB), value object boleh memiliki primary key. Namun primary key ini hanya digunakan untuk keperluan persistensi saja. Ketika sudah hydrate (diambil dari DB), dan dibuatkan object-nya lewat keyword `new`, berarti value object sudah masuk ke dalam logic flow bisnis/domain, primary key sudah tidak boleh digunakan.

### Attribute-Based Equality

Karena tidak memiliki ID, satu-satunya cara untuk menentukan dua buah value object sama atau tidak adalah kita cocokkan atributnya satu per satu. Jadi misalkan class tersebut memiliki 10 atribut. Berarti 10 atribut tersebut akan dicocokkan satu per satu.

Misalkan pada class **Money** sebelumnya, kita perlu menambahkan fungsi untuk mencocokkan object pada class ini. Karena di C# sudah ada fungsi `Equals` bawaan dari base class System.Object, kita lakukan override pada fungsi ini. Di C#, kita juga bisa melakukan override pada operator `==`. Bila kedepannya Anda menggunakan bahasa pemrograman lain, silakan disesuaikan. Yang penting kita menyediakan cara untuk mengecek equality sepasang obyek.

```csharp
public class Money
{
    public override bool Equals(object obj)
    {
        var m = obj as Money;
        if (m == null) return false; //jika obj bukan bertipe Money, langsung false

        //jika currency tidak sama, langsung false
        if (this.currency != m.currency) return false;

        //jika value tidak sama, langsung false
        if (this.value != m.value) return false;

        //jika semua field member sama, return true
        return true;
    }
}
```

Perhatikan code diatas. Kita lakukan typecast dulu pada variabel `obj` menjadi `Money`. Bila typecast gagal, berarti `obj` yang dioper bukanlah `Money`, fungsi langsung `return false`. Kemudian satu per satu atribut dicocokkan, bila ada satu saja yang tidak sama, maka `return false`.

Contoh penggunaannya:

```csharp
public static void Main(string[] args)
{
    Money m1 = new Money("IDR", 2500);
    Money m2 = new Money("IDR", 2500);
    Money m3 = new Money("IDR", 5000);

    Console.WriteLine(m1.Equals(m2)); //hasilnya true
    Console.WriteLine(m3.Equals(m2)); //hasilnya false
}
```

Berbeda dengan entity. Untuk mengecek 2 buah object entity sama atau tidak, cukup cek dari IDnya saja tanpa perlu mencocokkan atribut yang lain (akan dibahas di pertemuan selanjutnya: Entities).

### Behavior-Rich

Value object jangan sampai dibiarkan hanya bertugas untuk menyimpan atribut dan fungsi setter-getter.

Value object harus punya banyak behavior lainnya. Contohnya pada class **Money**, dapat ditambah fungsi untuk konversi ke mata uang lain.

Bila Anda membuat class `value object` yang tidak memiliki behavior, hati-hati masuk ke dalam smell [Data Class](https://sourcemaking.com/refactoring/smells/data-class).

### Cohesive

Hubungan antar method dan atribut di dalam class value object harus dibuat dengan tingkat kohesi setinggi mungkin. Kohesif sudah Anda dapatkan di matakuliah semester sebelumnya, Program Design Methods. Baca [link berikut](https://www.geeksforgeeks.org/software-engineering-coupling-and-cohesion/) untuk review kembali.

### Immutable

Arti harafiah dari immutable adalah: kekal, abadi. [link google translate](https://translate.google.com/#view=home&op=translate&sl=en&tl=id&text=immutable).

Untuk menjadikannya immutable, data di dalam value object tidak boleh diubah. Gampangnya, tidak boleh ada fungsi setter.

Sifat immutable ini akan membuat sistem menjadi lebih sulit untuk terkena bug. Manfaat immutable akan lebih terasa di building block Event Sourcing (pertemuan terakhir). Untuk gambarannya, dengan menjaga immutability dari value object, Anda akan mudah melakukan log, atau bahkan melakukan undo-redo state pada entity.

Method yang mirip dengan setter (berusaha mengganti value) diperbolehkan. Namun, method ini tidak boleh mengganti value dari atribut `this`. Method ini akan membuat object baru dengan value yang sesuai.

Misalnya kita ingin menyediakan fungsi `Add` dan `Substract` pada class **Money**:

```csharp
public class Money
{
    public Money Add(float value)
    {
        return new Money(this.currency, this.value + value);
    }

    public Money Substract(float value)
    {
        return new Money(this.currency, this.value - value);
    }
}
```

Perhatikan pada code diatas, method `Add` dan `Substract` tidak mengubah `this.value`. Kedua method tersebut mengembalikan obyek Money yang baru dengan value yang sudah dimodifikasi.

Contoh penggunaannya:

```csharp
public static void Main(string[] args)
{
    Money m1 = new Money("IDR", 2500);
    Money m2 = new Money("IDR", 2500);
    Money m3 = new Money("IDR", 5000);

    Console.WriteLine(m1.Equals(m2)); //true
    Console.WriteLine(m3.Equals(m2)); //false

    //true
    Console.WriteLine(m3.Substract(2000).Equals(m2.Add(500)));
}
```

### Combinable

Value seringkali direpresentasikan sebagai angka. Jadi bisa saja ada kebutuhan value object yang satu dapat digabungkan dengan value object lainnya dari class yang sama. Tapi ingat, ketika melakukan combine, immutability tetap harus dijaga.

Contohnya, kita bisa melakukan `Add` pada obyek **Money** dengan obyek **Money** lainnya.

```csharp
public class Money
{
    public Money Add(Money money)
    {
        if(this.currency != money.currency)
            throw new Exception("currency must be same");
        return new Money(this.currency, this.value + money.value);
    }
}
```

Perhatikan pada contoh code diatas, kita melakukan throw exception bila currency-nya tidak cocok. Pada implementasi yang lebih kompleks, bisa saja exception dihindari dengan cara: mata uang dikonversi dulu agar sama, kemudian baru value ditambahkan.

### Self-Validating

Value object tidak boleh dibiarkan dalam invalid state. Setiap kali di-create, value object harus selalu dalam keadaan valid. Keadaan valid yang dimaksud tentunya tergantung business requirement dari domain expert.

Self-validating dapat dilakukan di constructor ketika object pertamakali berusaha di-create. Bila ada data yang tidak valid, Anda bisa melempar Exception.

### Testable

Value object relatif mudah dibuat unit test-nya. Sebagai software engineer yang bertanggung jawab, Anda harus memaksimalkan test coverage di dalam codebase Anda.

Mungkin selama ini Anda biasanya melakukan pengetesan sistem dengan cara compile > run, kemudian Anda masukkan manual input dan melihat manual output yang diharapkan. Tentunya hal ini redundan, sangat memakan waktu, dan juga berbahaya karena faktor human-error besar.

Dengan menggunakan unit test, Anda membuat code untuk mengetes setiap line of code hal yang ingin Anda tes. Misal Anda membuat obyek menggunakan `new`. Kemudian dari code, Anda coba menjalankan sebuah method, kemudian Anda mengecek apakah output sudah sesuai dengan harapan. Dan begitu seterusnya. 

Untuk mendapatkan test coverage yang baik, Anda harus memastikan setiap line of code dalam class sudah dijalankan di dalam unit test. Semua corner case dalam business requirement juga harus Anda siapkan test case-nya.

Misalkan, kita buat beberapa test case untuk class **Money** diatas:

```csharp
[TestClass]
public class MoneyUnitTest
{
    [TestMethod]
    public void CreateExpectFailed()
    {
        try
        {
            Money m = new Money("a", 60000);
            Money m = new Money("abcde", 60000);
            Assert.Fail("expect exception");
        } catch (Exception) {}
    }

    [TestMethod]
    public void CreateExpectSuccess()
    {
        try
        {
            Money m = new Money("IDR", 60000);
        }
        catch (Exception e) {
            Assert.Fail(e.Message);
        }
    }

    [TestMethod]
    public void Add()
    {
        Money m1 = new Money("IDR", 60000);
        Money m2 = new Money("IDR", 30000);
        Money m3 = new Money("IDR", 90000);
        if (!m1.Add(m2).Equals(m3))
        {
            Assert.Fail("expect equals");
        }
    }
}
```

Agar tidak terlalu panjang, saya contohkan test case untuk memastikan Money gagal di-create bila currency tidak 3 char. Dan juga terdapat test case untuk memastikan fungsi Add versi combine berjalan dengan baik. Test coverage-nya tentu belum optimal karena kita belum cover method Substract dan juga method Add versi float.

Bila Anda belum mengetahui cara membuat unit test pada C#, coba ikuti [tutorial](https://docs.microsoft.com/en-us/visualstudio/test/walkthrough-creating-and-running-unit-tests-for-managed-code?view=vs-2019) ini.


## Persistence

### NoSQL

Anda bisa memilih teknologi berbasis NoSQL seperti MongoDB untuk menyimpan data value object langsung dengan format JSONnya ke dalam database.

### SQL

Anda juga bisa memakai tabel relasional yang selama ini pernah Anda pakai untuk di semester sebelumnya.

Bila Anda menyimpan dalam tabel relasional, ada dua pertimbangan bagaimana cara penyimpanannya:

1. Flat Denormalization

Data disimpan dalam satu tabel. Atribut-atribut dari class akan menjadi kolom-kolomnya. Bahkan jika memungkinkan, data disiapkan format khusus agar dapat disimpan cukup dalam satu kolom sehingga bisa dibaca lagi dengan mudah.

Contoh yang paling klasik adalah `DateTime`. Tentunya data `DateTime` tidak kita pisah-pisah kolomnya menjadi kolom tanggal, bulan, tahun, dan seterusnya.

Misalnya `DateTime` dapat kita simpan dengan format [RFC3339](https://tools.ietf.org/html/rfc3339). Contoh RFC3339: `2020-03-24T05:50:13+07:00`

2. Normalizing into Separate Tables

Denormalisasi adalah kondisi ideal untuk menyimpan value object agar tidak perlu query join yang memakan waktu.

Namun masih ada perusahaan yang butuh datanya berbentuk normal, misal sampai 4NF. Karena ini, value object terpaksa terpisah-pisah ke dalam beberapa table.

Seperti yang kita sudah bahas sebelumnya, untuk kasus persistensi (penyimpanan ke DB), value object boleh memiliki primary key. Untuk kasus value object yang terpisah dari entity-nya. Berarti table value object tersebut tinggal kita berikan foreign key ke table entity.
