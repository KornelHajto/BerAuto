# BerAuto
Rendszerfejlesztés haladó módszerei nevű tárgy beadandó feladatának repoja
## Beadandó 1 - Leírás

A modellek mind egy class library-ben találhatóak, amiatt, hogy a frontend és az api is ugyanazokkal a modellekkel dolgozzon, és ne kelljen többször ugyanazokat megírni.
Az egész API és DB dockeren keresztül fut. Az API amint elindul ha még nincsenek a db-ben meg a migrációk automatikusan lefutnak (Ez megtalálható a program.cs fájlban és a MigrationExtension.cs fájlban).

## Követelmények

- **Visual Studio 2022** vagy újabb
- **Docker Desktop**: A backend és az adatbázis Docker konténerekben fut.
