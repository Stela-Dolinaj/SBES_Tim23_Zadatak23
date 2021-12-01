Projektni zadatak 23.

    Implementirati sistem za rad sa udaljenom bazom podataka.    
    Klijenti se sa bazom, kao i međusobno, autentifikuju putem sertifikata.
U bazi je potrebno da ima bar 3 različite datoteke u koju će klijenti da upisuju podatke. Ove
datoteke treba da budu namenjene tipu SCADA uređaja koji šalje podatke. Potrebno je takođe
simulirati podatke sa bar 2 uređaja svakog tipa (min. 6 uređaja) čiji će podaci sadržati vremensku
oznaku, korisničko ime uređaja, grupu kojoj uređaj pripada, oznaku merne jedinice i njenu
vrednost.
    
    Klijenti istog tipa treba međusobno da komuniciraju da bi sprečili nekonzistentnost baze
podataka. Pre slanja podataka u bazu, ali i u trenutku kad završi slanje, svaki klijent treba da javi
ostalim klijentima istog tipa da je slanje podataka ka bazi zabranjeno i u tom periodu niko sem
njega ne sme da šalje podatke ka bazi, odnosno da po završetku slanja dozvoli ostalim klijentima
da šalju podatke.
    
    Klijenti se autentifikuju putem sertifikata moraju da izvrše validaciju sertifikata na osnovu lanca
poverenja, dok je autorizacija zasnovana na pripadnosti grupi.
Dodatno, sve poruke u sistemu moraju biti digitalno potpisane.
Sistem treba da loguje sve uspešne i neuspešne autentifikacije i autorizacije u Windows Event
Log-u.
    
    Napomena: U okviru sertifikata u SubjectName treba upisati za CN korisnicko ime, a za OU
grupu kojoj korisnik pripada