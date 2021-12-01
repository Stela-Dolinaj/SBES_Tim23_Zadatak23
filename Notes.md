## Projekat Notes

- Korisnici: (Computer Managment)
  * server (service1)
  * client (scadaClient x 6)
      - scadaPressure1
      - scadaPressure2
      - scadaTemperature1
      - scadaTemperature2
      - scadaVoltage1
      - scadaVoltage2

- Autentifikacija => PUTEM SERTIFIKATA, CHAIN TRUST validacija sertifikata (vezba 3)
    - medju klijentima
    - izmedju klijenta i baze(servera)

- Sertifikati:
  * Napomena: U okviru sertifikata u SubjectName treba upisati za CN korisnicko i me, a za OU grupu kojoj korisnik pripada
    * DOPUNI POTREBNE SERTIFIKATE

- U bazi postoje 3 razlicite datoteke u koju se upisuju podaci odgovarajuceg tipa:
  * pressure.json
  * temperature.json
  * voltage.json
    - PODATAK:
      * vremenska oznaka
      * korisnicko ime uredjaja (scadaPressure1..)
      * grupa kojoj uredjaj pripada
      * oznaka merne jedinice
      * vrednost

- ! Klijenti istog tipa, pre slanja podataka u bazu treba da obaveste ostale klijente iste grupe, da je upis u bazu zabranjen. Nakon upisa u bazu, potrebno ih je obavestiti da je dozvoljen nastavak koriscenja baze.

- Autorizacija => ZASNOVANA NA PRIPADNOSTI GRUPI

- Sve poruke u sistemu moraju biti DIGITALNO POTPISANE

- Sistem treba da loguje sve USPESNE i NEUSPESNE AUTENTIFIKACIJE i AUTORIZACIJE u Windows Event Log-u

