# 🚀 Simple DDoS IDS using MLP

Acest proiect a avut ca scop crearea unei retele neuronale simple (MLP), capabila sa detecteze un posibil atac DDoS. Ulterior, in jurul retelei am construit o aplicatie in C#, similara unui IDS.

EN: This project aimed to create a simple neural network (MLP), capable of detecting a possible DDoS attack. Subsequently, we built an application in C#, similar to an IDS, around the network.
---

## Cuprins

- [Despre proiect](#despre-proiect)
- [Functii](#functii)
- [Screenshots](#screenshots)
- [Usage](#usage)
- [Bibliografie/Referinte](#bibliografiereferinte)

---

## Despre proiect

Acest proiect implementează un sistem de detecție a atacurilor DDoS folosind o rețea neuronală de tip MLP (Multi-Layer Perceptron). Sistemul este capabil să detecteze pachete de rețea malițioase pe baza a 18 caracteristici extrase din trafic, fiind antrenat pe datasetul CICIDS 2017 și compatibil cu APA-DDoS.

Scopul este oferirea unei soluții simple și rapide pentru identificarea atacurilor DDoS în rețele de calculatoare.

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

1. Exportă datele de trafic din Wireshark folosind un script Python ce extrage cele 18 caracteristici.
2. Asigură-te că fișierul Excel are aceleași coloane ca în setul de antrenament.
3. Rulează aplicația și selectează fișierul .xlsx.
4. Aplicația va returna 0 (normal) sau 1 (DDoS).

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

