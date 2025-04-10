# 🚀 Simple DDoS IDS using MLP

A lightweight DDoS Intrusion Detection System based on a Multi-Layer Perceptron (MLP) neural network. The network is trained on the CICIDS 2017 dataset and supports real-time testing via packet data captured and preprocessed externally.

---

## Cuprins

- [Despre proiect](#despre-proiect)
- [Functii](#functii)
- [Screenshots](#screenshots)
- [Usage](#usage)
- [Bibliografie/Referinte](#bibliografiereferinte)
- [Contact](#contact)
- [Licenta](#licenta)

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

