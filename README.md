# StudentDatabaseTUI

| Nama                        | NRP        | Kelas  |
| --------------------------- | ---------- | ------ |
| Mohamad Valdi Ananda Tauhid | 5025221238 | PBKK-C |

Source Code: [valdi-at/pbkk-c-student-database-tui](https://github.com/valdi-at/pbkk-c-student-database-tui)

Pada tugas ini saya membikin aplikasi desktop database mahasiswa sederhana yang jalan di terminal (TUI = Text User Interface). Aplikasi ini dapat melihat semua data, cari, tambah, update, hapus data mahasiswa, dan bisa ganti bahasa antara english sama indonesia pada saat aplikasi berjalan.

## .NET framework

Project ini dibuat menggunakan C# .NET framework, tepatnya target `net10.0` (StudentDatabaseTUI.csproj). 

Aplikasi .NET sendiri itu intinya runtime + sekumpulan library yang membikin kita bisa menulis kode menggunakan C# dan di-compile menjadi suatu yang bisa dijalanin sama runtime-nya. File `.csproj` adalah project file, isinya memberikan informasi kepada SDK mengenai jenis projeck ini itu apa (`OutputType=Exe` artinya console app, bukan merupakan library), target framework versi berapa, dan beberapa switch compiler:

Untuk menjalankan program, kita dapat melakukanya menggunakan `dotnet run` di folder project, dan `dotnet build` jika hanya ingin melakukan compile saja tanpa dijalanin. SDK akan menguruskan restore package, compile, sampai membikin exe-nya pada `bin/`.

Dikarenakan saya menggunakan Linux, saya menginstall .NET menggunakan:
``` bash
sudo pacman -S dotnet-sdk
```

Setelah itu kita menggunakan `C# Dev Tools` di Visual Studio Code
![](https://docs.shinyshoe.net/uploads/d98f026f-cfc3-4283-beb2-90da6554fe8e.png)


## Cara kerja aplikasinya secara umum

![](https://docs.shinyshoe.net/uploads/84b62ce8-3a0c-4619-a8ee-1a5bf67a857a.png)

Inti dari aplikasi ini adalah sebuah loop. pada saat aplikasi dijalanin aplikasi melakukan print menu (angka 1-6 untuk operasi mahasiswa, `l` untuk ganti bahasa dan `q` untuk keluar), jika kita ketik sebuah pilihan, aplikasinya akan melakukan sebuah function yang sesuai, dan akan mencetak menu lagi. Ini akan  berulang sampai kita memilih `q`.

Menu-nya sendiri tidak di-hardcode mengunakan if/else, melainkan akan dibikin sebagai `Dictionary<string, (string description, Action action)>` di Program.cs (pada `BuildMenuItems()`), jadi setiap key menu itu mengarh ke teks deskripsi (yang sudah diterjemahin) sama action (function yang dipanggil). Ini di-rebuild setiap iterasi loop, jadi jika kita ganti bahasa, teks menunya ikut update juga.

Seperti menu utama, pemilih bahasa (`l`) juga kerja dengan cara yang sama, dia membikin menu kecil sendiri secara dinamis dari bahasa apa aja yang ketemu, kita pilih angkanya, terus dia memanggil `i18n.SetLanguage()`.

Untuk menyimpan data mahasiswa, aplikasinya hanya baca/tulis ke file `students.json` yang ada di sebelah exe-nya, tidak ada database engine beneran di belakangnya.

## Struktur aplikasi

```
StudentDatabaseTUI/
├── Program.cs                # entry point, loop menu, semua "action"
├── sdb/
│   ├── Student.cs            # model Student
│   ├── StudentManager.cs     # logic CRUD + simpen/baca json
│   └── I18n.cs               # load translasi + lookup
├── lang/
│   ├── en_US.json            # translasi bahasa inggris
│   └── id_ID.json            # translasi bahasa indonesia
├── students.json             # data mahasiswa asli (dibuat/diupdate pada saat runtime)
└── StudentDatabaseTUI.csproj
```

Folder `sdb` itu singkatan dari "student database", semua yang bukan logic UI/menu ada di situ, di bawah namespace `StudentDatabaseTUI.sdb`. Pada [Program.cs] ditulis menggunakan top-level statements (tidak menggunakan boilerplate `class Program { static void Main() }`), jadi seluruh file jalan dari atas ke bawah, dengan local function seperti `ViewAllStudents()` dan `AddStudent()` dideklarasikan di bagian bawah terus dipanggil dari loop di atasnya.

## Class

### Student

Hanya model data biasa, dua properti, `Name` sama `Id`, keduanya string.

### StudentManager

Ini tempat semua logic CRUD (Create, Read, Update, Delete). dia menyimpan `List<Student>` di memory sama `filePath` yang menunjuk ke file json-nya. pada saat di-construct dia memanggil `LoadStudents()` yang baca file jika ada (menggunakan `JsonSerializerOptions { PropertyNameCaseInsensitive = true }` biar key json-nya tidak harus sama persis casing-nya seperti properti C#), atau jika filenya belum ada, ya mulai dari list kosong aja.

method publicnya cukup jelas dari namanya: `GetAllStudents()`, `FindByName()` (case insensitive), `FindById()`, `AddStudent()`, `UpdateStudent()`, `DeleteStudent()`. setiap method yang ngubah list (add/update/delete) langsung memanggil `SaveStudents()` abis itu, yang hanya nge-serialize ulang seluruh list ke disk menggunakan `WriteIndented = true`. jadi tidak ada tombol "save" terpisah, setiap perubahan langsung ke-persist.

### I18n

I18n(singkatan dari Internationalization) adalah class yang akan menghandalkan translasi aplikasi. Aplikasi ini dijekaskan lebih detail di bagian bawah, tapi versi singkatnya adalah: dia load semua file `.json` di folder `lang` ke memory, menyimpan bahasa mana yang lagi "current", dan punya method `Get(key)` yang me-resolve key bertitik seperti `"menu.viewAllStudents"` jadi string terjemahan aslinya.

## Fitur i18n

Fitur ini intinya membikin aplikasi bisa nampilin teks dalam beberapa bahasa tanpa hardcode string di mana-mana. Aplikasi ini support itu dengan meletakan semua teks tampilan di luar kode, di dalem file json di `lang/`, satu file per bahasa.

Setiap file lang bentuknya seperti gini (lihat `lang/en_US.json` sama l`ang/id_ID.json`):

```json
{
  "name": "Bahasa Indonesia",
  "translation": {
    "menu": { "viewAllStudents": "Lihat Semua Siswa", ... },
    "messages": { "noStudentsFound": "Tidak ada siswa ditemukan.", ... },
    "prompts": { "enterStudentName": "Masukkan nama siswa: ", ... }
  }
}
```

File `I18n.cs` load semua file json yang berada di folder `lang` pada saat startup (`LoadAllLanguages()`), di-key menggunakan nama filenya (jadi `id_ID.json` jadi language id `id_ID`). pada saat kita memanggil `i18n.Get("menu.viewAllStudents")`, dia split key-nya menggunakan `.` terus jalan turun ke nested json object part demi part (`menu` terus `viewAllStudents`) sampai ketemu string, atau jika di tengah jalan ada part yang tidak ketemu, dia langsung return key-nya sendiri sebagai fallback (jadi tidak crash, paling parah kita liat raw key di layar bukan teks terjemahannya, yang mana ini juga kepake untuk debug translasi yang hilang).

Ganti bahasa hanya memanggil `SetLanguage(langId)`, dia tinggal menukar `currentTranslations` biar menunjuk ke dictionary yang sudah keload untuk bahasa itu, tidak ada baca file ulang atau apapun. menu pemilih bahasa di `Program.cs` (`ChooseLanguage()`) dibikin dengan memanggil `i18n.GetAvailableLanguages()` (return semua language id yang keload) sama `i18n.GetLanguageName()` untuk nama tampilannya, jadi nambahin bahasa baru, jadi hanya tinggal menarukan file json baru di `lang/`, tidak perlu merubah kode sama sekali.

Satu hal yang perlu dihingat, default language yang dipassing ke `new I18n("lang", "en_US")` di `Program.cs` itu harus sama persis seperti nama filenya (case sensitive di beberapa sistem), jadi jika kita rename `en_US.json`, jangan lupa update itu juga.
