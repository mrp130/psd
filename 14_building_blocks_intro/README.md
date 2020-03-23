## Building Blocks

Di pertemuan-pertemuan tatap muka sebelumnya, kita sudah membahas strategic patterns dari DDD. Sampai akhir semester, kita akan membahas tactical patterns dari DDD atau dikenal dengan building blocks. Tactical patterns di Buku Scott Millett dimulai dari bab 14.

Building blocks yang dibuat Eric Evans tidak sepenuhnya baru. Anda bisa melihat banyak pattern dari Eric Evans diambil dari buku Martin Fowler berjudul Patterns of Enterprise Application Architecture. Software engineer kerap menyebut buku sakti ini sebagai PoEAAA. Martin Fowler membuat rangkuman dari katalog buku ini [disini](https://www.martinfowler.com/eaaCatalog/). Selain PoEAAA, banyak juga prinsip yang diambil dari Gang of Four Design Pattern, software engineer penulis dan buku sakti ini sebagai GoF. GoF akan dipelajari semester depan. Jadi, walaupun perusahaan Anda nanti tidak sepenuhnya mengimplementasi DDD, Anda tetap dapat mengadopsi cara Eric Evans untuk merancang bagian-bagian codenya.

Eric Evans dan Scott Millett menekankan bahwa pendekatan taktikal kita untuk merancang software akan terus berkembang. Akan ada building block baru. Definisi dari building block yang akan kita bahas juga bisa saja berubah. Yang paling penting adalah kita menjaga Ubiquitous Language agar building block dapat dipahami dengan arti yang sama untuk semua software engineer dalam tim.

![Building blocks](img/building_blocks.PNG)

Building blocks yang akan kita bahas dari buku Scott Millett adalah:

- [Value Objects](../15_value_objects)
- Entities
- Domain Services
- Domain Events
- Aggregates
- Factories
- Repositories
- Event Sourcing