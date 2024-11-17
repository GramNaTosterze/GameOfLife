Conway's Game of Life implementation in C# and WPF

Requirements:
- [X] Aplikacja powinna zostać zrealizowana w WPF
- [X]    Aplikacja powinna umożliwiać zadanie rozmiaru planszy (100x100 lub więcej)
- [X] edycje stanu
- [X] czyszczenie i losowanie nowego stanu
- [X] zapis/odczyt stanu do pliku
- [X] wykonywanie pojedynczego kroku i ciągłą animację stanu automatu
- [X] regulacja prędkości
- [X] proszę umożliwić dostęp do statystyki od początku, liczba pokoleń/ile komórek umarło/ile się urodziło)
- [X] Dwupoziomowy zoom (duże komórki i małe (przy dużych widoczny jest fragment planszy ale cała „żyje”)
- [X] Zmiana prezentacji komórek, pełna lub kolko, kolor
- [?] W aplikacji należy pokazać typowe koncepcje WPF: style, szablony, triggery, animacje itd

Additional:
- [?] wielkie automaty (>1000),
- [X] wielopoziomowy zoom
- [ ] zapis do pliku  obrazu lub sekwencji obrazów lub filmu
- [ ] konfiguracje reguł automatu w nast. postaci po literze B liczb sąsiadów dającej narodziny, a następnie po ukośniku i literze S liczb sąsiadów dającej przeżycie. Reguły Conwaya zapisuje się wtedy jako: B3/S23,
- [ ] wybór ciekawych konfiguracji np. na podstawie - https://pl.wikipedia.org/wiki/Gra_w_%C5%BCycie
- [ ] zaimplementowanie wybranych modeli kolorowania np. https://pl.wikipedia.org/wiki/Gra_w_%C5%BCycie#Immigration lub
- [ ] https://pl.wikipedia.org/wiki/Gra_w_%C5%BCycie#QuadLife
- [ ] interakcja z „żywą” planszą – w trakcie rysowania (przycisnięty przycisk mysz generowanie jest przerywane, po puszczeniu przycisku myszy wznawiane)
  - [ ]   automat na innej siatce trójkatna/szesciokątna (ocena do negocjacji :-))
