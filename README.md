# 🚀 Simple DDoS IDS using MLP

RO: Acest proiect a avut ca scop crearea unei retele neuronale simple (MLP), capabila sa detecteze un posibil atac DDoS. Ulterior, in jurul retelei am construit o aplicatie in C#, similara unui IDS.

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

Atentie! Aceasta aplicatie nu este menita sa simuleze un IDS functional 100%, nici o retea neuronala sofisticata. Am vrut sa vedem care este minimul necesar d.p.d.v. software (resurse, arhitectura) pentru detectia unui astfel de atac. Astfel, este posibil sa intalnim atat rezultate bune, cat si deficitare (mai ales ca setul de date este creat de la 0).

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
- posibilitate de salvare și încărcare ulterioară a modelului
- testare pe date fără label
- output binar 0 (normal) sau 1 (atac DDoS) -> clasificare

  
---

## Screenshots


---

##  Usage

1. Exportă datele de trafic din Wireshark folosind un script Python ce extrage cele 10/6 caracteristici.
2. Asigură-te că fișierul Excel are aceleași coloane ca în setul de antrenament.
3. Rulează aplicația și selectează fișierul .xlsx.
4. Aplicația va returna 0 (normal) sau 1 (DDoS).

---

## Rezultate

---

### Viitor


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

