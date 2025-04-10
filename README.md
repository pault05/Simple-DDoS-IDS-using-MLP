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
- [Bibliografie/Referinte](#bibliografiereferinte)

---

## Despre proiect

Acest proiect implementează un sistem de detecție a atacurilor DDoS folosind o rețea neuronală de tip MLP (Multi-Layer Perceptron). Sistemul este capabil să detecteze pachete de rețea malițioase pe baza unor caracteristici cheie din pachetele care constituie traficul de retea. Vom nota cu 1 pachetele (liniile) malitioase, malgine, și cu 0 cele benigne.

Atentie! Aceasta aplicatie nu este menita sa simuleze un IDS functional 100%, nici o retea neuronala sofisticata. Am vrut sa vedem care este minimul necesar d.p.d.v. software (resurse, arhitectura) pentru detectia unui astfel de atac.

Setul de date este rezultatul procesării datelor colectate în urma unui experiment rulat în Lab. 301 (CISCO Lab), din cadrul UMFST Tg. Mures. Am folosit hping3 pentru a simula 3 tipuri majore de atac DoS:

## 1. TCP Syn Flood - atac care epuizeaza resursele victimei
## 2. UDP Flood - atac volumetric
## 3. ICMP Flood - atac volumetric, bazat pe ping

## Script atac

#!/bin/bash
	# ICMP:
hping3 -i u10000 -c 100000 -1 192.168.0.201
	# TCP:
hping3 -S -p 80 --flood 192.168.0.201
	# UDP:
hping3 --udp --flood --rand-source -p 53 192.168.0.201



---

## Etape

## 1.

## 2. 

---

##  Functii

- ✅ Detectarea traficului DDoS pe baza caracteristicilor de rețea
- 📄 Suport pentru fișiere .xlsx ca input
- 🔎 Analiză batch și testare în timp real (cu pachete preprocesate)
- 📊 Returnează eticheta 0 (normal) sau 1 (atac DDoS)

---

## Screenshots

| Interfață | Descriere |
|-----------|----------|
| ![screenshot1](images/gui_example.png) | ex1 |
| ![screenshot2](images/result_example.png) | ex 2|

---

##  Usage

1. Exportă datele de trafic din Wireshark folosind un script Python ce extrage cele 10/6 caracteristici.
2. Asigură-te că fișierul Excel are aceleași coloane ca în setul de antrenament.
3. Rulează aplicația și selectează fișierul .xlsx.
4. Aplicația va returna 0 (normal) sau 1 (DDoS).

---

## Rezultate

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

