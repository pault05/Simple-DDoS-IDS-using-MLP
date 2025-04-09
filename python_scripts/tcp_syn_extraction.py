from scapy.all import rdpcap, IP, TCP
import pandas as pd
import sys

# extragerea caracteristicilor
def extract_features_from_pcap(pcap_file):
    packets = rdpcap(pcap_file)  # pachetele din fisierul original
    data = []

    for packet in packets:
        if packet.haslayer(IP) and packet.haslayer(TCP):  # verificare TCP
            ip_layer = packet[IP]
            tcp_layer = packet[TCP]

            # SYN fara ACK
            is_syn_flood = 0
            if tcp_layer.flags == 'S':  # SYN flag
                is_syn_flood = 1  # malign packet

            features = {
                'src_ip': int(ip_layer.src.replace('.', '')),   # ip-urile sunt int
                'dst_ip': int(ip_layer.dst.replace('.', '')),
                'src_port': tcp_layer.sport,
                'dst_port': tcp_layer.dport,
                'seq': tcp_layer.seq,
                'ack': tcp_layer.ack,
                'window': tcp_layer.window,
                'flags': int(tcp_layer.flags),  # flag-uri -> int
                'packet_size': len(packet),
                'ttl': ip_layer.ttl,
                'label': is_syn_flood
            }
            data.append(features)

    return pd.DataFrame(data)

def main():
    if len(sys.argv) != 3:
        print("usage: python script.py <pcap_file> <output_csv>")
        sys.exit(1)

    pcap_file = sys.argv[1]
    output_csv = sys.argv[2]

    df = extract_features_from_pcap(pcap_file)
    df.to_csv(output_csv, index=False)
    print("CSV file saved : {output_csv}")

if __name__ == "__main__":
    main()
