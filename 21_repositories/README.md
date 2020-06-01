# Repositories

Tulisan ini adalah rangkuman dari bab 20 buku Scott Millett, serta ditambahkan dengan beberapa informasi lainnya. Harap mahasiswa membaca lagi bab tersebut setelah membaca rangkuman ini karena banyak contoh code lain di dalam buku yang dapat membantu lebih memahami Factories.

---

Code sebelum ditambahkan: https://github.com/mrp130/psd/tree/4-factory

---


### Update Database

1. Pastikan `TargetFramework` minimal netstandard2.0 pada file [Game.csproj](../Xyz/Game/Game.csproj)

```xml
<PropertyGroup>
  <TargetFramework>netstandard2.0</TargetFramework>
</PropertyGroup>
```

2. Add package `Npgsql.EntityFrameworkCore.PostgreSQL` dengan cara menjalankan command berikut. Pastikan Anda menjalankan command di dalam folder Game.

```
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```
