# 🚀 Simple DDoS IDS using MLP

RO: Acest proiect a avut ca scop crearea unei rețele neuronale simple (MLP), capabilă să detecteze un posibil atac DDoS. Ulterior, în jurul rețelei am construit o aplicație în C# .NET, similară unui IDS.

EN: This project aimed to create a simple neural network (MLP), capable of detecting a possible DDoS attack. Subsequently, we built an application in C#, similar to an IDS, around the network. This project aims to show that even a simple model can help us in detecting and mitigaiting these kind of attacks. 

---

## Cuprins

- [Despre proiect](#despre-proiect)
- [Etapele proiectului](#etape)
- [Functii](#functii)
- [Screenshots](#screenshots)
- [Usage](#usage)
- [Rezultate](#rezultate)
- [Viitor](#viitor)
- [Bibliografie/Referinte](#bibliografiereferinte)

---

## Despre proiect

Acest proiect implementează un sistem de detecție a atacurilor DDoS folosind o rețea neuronală de tip MLP (Multi-Layer Perceptron). Sistemul este capabil să detecteze pachete de rețea malițioase pe baza unor caracteristici cheie din pachetele care constituie traficul de retea. Vom nota cu 1 pachetele (liniile) malitioase, malgine, și cu 0 cele benigne.

Atentie! Aceasta aplicatie nu este menita sa simuleze un IDS functional 100%, nici o retea neuronala sofisticata. Am vrut sa vedem care este minimul necesar d.p.d.v. software (resurse, arhitectura) pentru detecția unui astfel de atac. Astfel, este posibil să întălnim atât rezultate bune, cât și deficitare (mai ales că setul de date este creat de la 0; operăm cu numere mari -> fragmente, overfitting...).

Setul de date este rezultatul procesării datelor colectate în urma unui experiment rulat în Lab. 301 (CISCO Lab), din cadrul UMFST Tg. Mures. Am folosit hping3 pentru a simula 3 tipuri majore de atac DoS:

 1. TCP Syn Flood - atac care epuizeaza resursele victimei
 2. UDP Flood - atac volumetric
 3. ICMP Flood - atac volumetric, bazat pe ping

## Script atac

#!/bin/bash

	# ICMP:
 
hping3 -i u10000 -c 100000 -1 192.168.0.201

	# TCP:
 
hping3 -S -p 80 --flood 192.168.0.201

	# UDP:
 
hping3 --udp --flood --rand-source -p 53 192.168.0.201

Acest script a fost rulat simultan de restul PC-urilor din rețea.

## Topologia retelei 

![screenshot1](images/A-301-victim.png)

Ținta atacului este PC-ul cu adresa IP: 192.168.0.201.

Am capturat atăt trafic curat, fără flooding, dar și 3 capturi aferente fiecărui tip de atac. Am utilizat Wireshark și, ulterior, am prelucrat fișierele utilizând scripturi Python (scapy, pandas).
Cu ajutorul acestora am:

 - extras și etichetat date aferente fiecărui tip de atac
 - amestecat datele cu trafic curat (pe blocuri de pachete și uniform)

## Strucutura datelor extrase

1. TCP - SYN Flood
   - src_ip : adresă IP sursă
   - dst_ip : adresă IP destinație
   - src_port : port sursă
   - dst_port : port destinație
   - seq: nr. de secvență (ordine pachete)
   - ack: acknowledgment rumber (confirmare de primire pachete)
   - window: mărimea ferestrei în care se pot primi date (pana la un nou ACK)
   - flags: diverse flag-uri pentru starea conexiunii TCP (ne interesează flag-ul 2 -> SYN)
   - packet_size: mărimea pachetului
   - ttl: time to live (câte hop-uri poate traversa un pachet)
   - label: 1 sau 0

3. UDP Flood
   - src_ip : adresă IP sursă
   - dst_ip : adresă IP destinație
   - src_port : port sursă
   - dst_port : port destinație
   - packet_size: mărimea pachetului
   - ttl: time to live (câte hop-uri poate traversa un pachet)
   - label: 1 sau 0

4. ICMP Flood
   - src_ip : adresă IP sursă
   - dst_ip : adresă IP destinație
   - packet_size: mărimea pachetului
   - ttl: time to live (câte hop-uri poate traversa un pachet)
   - ip_flags: flag pentru header-ul IP (DF sau MF)
   - fragment_offset: aparține sau nu pachetului original
   - ip_header_length: mărimea header-ului IP
   - ip_checksum: posibile pachete malformate
   - src_mac: adresă MAC sursă
   - dst_mac: adresă MAC destinație
   - label: 1 sau 0

Am obtinut fișiere Excel care vor servi rețeaua neuronală cu date de antrenament (70%) și de test (30%).

## Arhitectura rețelei neuronale

- stratul de intrare (cu nr. de neuroni egal cu numărul de coloane - caracteristici - din fișierul .excel)
- un strat ascuns (funcția de activare Sigmoid sau TanH, la alegere) (nr. de neuroni la alegere)
- stratul de ieșire (0 sau 1)

Aplicația conține :
- clasa Neuron
- clasa Neural_Network
- clasa principală, unde am definit restul funcțiilor, metodelor, grafice, visuals, etc.
  
---

## Etape
1. Am stabilit ce fel de atac dorim să detectăm -> DDoS.
2. Am stabilit tipurile de DoS (TCP-SYN, UDP, ICMP)
3. Am stabilit parametrii experimentului (hping3, topologie, etc.) și am colectat datele (Wireshark).
4. Am prelucrat datele -> scripturi Python -> am obținut fișiere Excel cu date și label-uri.
5. Am stabilit arhitectura rețelei (simplă, orientată spre eficiență și demonstrație).
6. Am creat rețeaua neuronală, elementele vizuale, funcționalități, grafice.
7. Am antrenat și testat aplicația / rețeaua neuronală.
8. Am colectat datele.

---

##  Functii

- detectarea traficului DDoS pe baza caracteristicilor de rețea / pe baza pachetelor 
- suport pentru fișiere .xlsx ca input
- posibilitate de alegere a parametriilor (nr. neuroni strat ascuns, nr. epoci, eroare maximă, rata de învățare)
- feedback asupra procesului de antrenare (per epocă)
- confusion matrix și grafic pentru observarea tiparului de atac (liniar, spike)
- atenționare în cazul detecției mai multor spike-uri (mai multe pachete maligne într-un segment de trafic)
- funcție de logging -> fișiere .txt cu detalii despre atac / pachete.
- posibilitate de salvare și încărcare ulterioară a modelului
- testare pe date fără label
- output binar 0 (normal) sau 1 (atac DDoS) -> clasificare

---

## Screenshots

Ecranul de start al aplicației.
![screenshot2](images/ecran-start.png)

Încărcarea datelor și normalizarea lor, dupa funcția de activare aleasă.
![screenshot3](images/incarcare-normalizare.png)

Procesul de antrenare.
![screenshot4](images/antrenare.png)

Atenționare, afișată după procesul de testare.
![screenshot5](images/atentionare.png)

Rezultatele testării.
![screenshot6](images/rezultate-test.png)

Vizualizare rezultate testare.
![screenshot7](images/date-test.png)

Cum arată fișierul cu modelul salvat (nr. neuroni intrare, strat ascuns, strat ascuns->ieșire).
![screenshot8](images/model-salvat.png)

Spikes
![screenshot9](images/spikes.png)

Consecutive Spikes
![screenshot10](images/consecutive-spikes.png)


---

##  Usage

1. Colectăm trafic de rețea cu Wireshark.
2. Prelucrăm fișierul .pcap cu scriptul aferent din Python (în funcție de ce atac vrem să detectăm).
3. Încărcăm fișierul în aplicație. Atentie! Numărul de neuroni din startul de intrare trebuie să fie egal cu nr. de coloane din excel - 1 (coloana de label) -> setare din cod.
4. Alegem funcția de activare.
5. Normalizăm datele.
6. Setăm parametrii.
7. Antrenăm rețeaua.
8. Testăm.
9. Vizualizăm rezultatele / fișierele de logging.
10. Salvăm rețeaua.
11. Testăm cu date reale.

---

## Rezultate

Rezultatele sunt împărțite după: 
- tipul / nr. de date oferite
- aranjarea pachetelor (segmente sau uniforme)
- parametrii folosiți

Toate fișierele au conținut aproximativ 5000 de pachete.


TOATE COLOANELE / DATELE

Parametrii: 
 - 15 neuroni pe stratul ascuns
 - 300 epoci
 - rata învățare: 0,005
 - eroare maximă: 0,001

Blocuri de câte 50 pachete:
1. tcp-syn: 36.63 % din pachete sunt maligne
- sigmoid: 99.73%
- tanH: 94.25% cu rata: 0,0000005 si eroarea: 0,000001

2. imcp: 25.30% din pachete sunt maligne
- sigmoid: 86.37%
- tanH: 92.31%

3. udp: 53.32% din pachete sunt maligne
- sigmoid: 99.59%
- tanH: 99.80%


Trafic uniform:
1. tcp-syn: 37.45% din pachete sunt maligne
- sigmoid: 99.66%
- tanH: 92.65% cu rata 0,0000005 si eroarea: 0,0000001

2. icmp: 24.82% dn pachete sunt maligne
- sigmoid: 86.20%
- tanH: 95.42%

3. udp: 52.83% din pachete sunt maligne
- sigmoid: 98.65%
- tanH: 99.46%


NO IP & MAC

Parametrii: 
- 10 neuroni pe stratul ascuns
- 200 epoci
- rata învățare: 0,005
- eroare maximă: 0,001

Blocuri de câte 50 pachete
1. tcp-syn: 36.63 % din pachete sunt maligne		
- sigmoid: 99.73%
- tanH: 99.86% cu rata: 0,000005 si eroarea: 0,000001

2. imcp: 25.30% din pachete sunt maligne
- sigmoid: 73.68%
- tanH: 89.07%

3. udp: 53.32% din pachete sunt maligne
- sigmoid: 79.14%
- tanH: 98.85%


Trafic uniform
1. tcp-syn: 37.45% din pachete sunt maligne
- sigmoid: 99.66%
- tanH: 90.38% cu rata: 0,0000005 si eroarea: 0,0000001

2. icmp: 24.82% din pachete sunt maligne
- sigmoid: 75.08% 
- tanH: 95.49%

3. udp: 52.83% din pachete sunt maligne
- sigmoid: 80.00%
- tanH: 97.98%


În general, am obținut rezultate satisfăcătoare, având în vedere arhitectura simplă a rețelei, dar și structura datelor.
Observăm și câteva probleme:
-  în cazul funcției TanH, pentru detecția SYN Flood, avem nevoie de parametrii foarte mici
-  în unele configurații, am obținut acuratețe de 100.00%, dar testarea nu avea rezultatele așteptate
-  în unele cazuri, la testarea reală, am obținut doar label-uri de 1 -> overfitting

---

### Viitor

Principala direcție de viitor a proiectului este extinderea și eficientizarea acestuia, în cadrul studiilor de masterat, prin:

1. îmbunătățirea rețelei neuronale -> alți algoritmi, biblioteci predefinite (pandas, ML, scikit, jupyter, etc.)
2. mai mulți parametrii pentru pachete / trafic
3. detectare atac în timp real -> colectare date din NIC
4. interfață mai bună
5. firewall -> blocare IP / MAC / PORT cu trafic malign
6. extinderea aplicației într-o soluție dedicată și funcțională -> un IDS cu mai multe funcționalități
7. server dedicat unde putem vizualiza situația unei scanări, stocare raport, etc.
8. multi-platformă: desktop, mobile, web

---

## Bibliografie/Referinte

Network Intrusion Detection System Using Anomaly Detection Techniques,
David Oroian, Roland Bolboaca, Virgil Dobrota, Adrian-Silviu Roman

https://ieeexplore.ieee.org/document/9282658/figures#figures

https://ieeexplore.ieee.org/document/9726747

https://www.kaggle.com/datasets/devendra416/ddos-datasets

https://www.unb.ca/cic/datasets/ids-2017.html

https://github.com/noushinpervez/Intrusion-Detection-CICIDS2017

https://github.com/steviegoneevil/ANN-for-DDoS-detection

https://github.com/ReubenJoe/DDoS-Detection

