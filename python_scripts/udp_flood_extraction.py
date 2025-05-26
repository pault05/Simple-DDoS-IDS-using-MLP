from scapy.all import rdpcap, IP, UDP, TCP
import pandas as pd
import sys

def extract_features_from_pcap(pcap_file):
    packets = rdpcap(pcap_file) # pachetele din fisierul original
    data = []

    for packet in packets:
        if packet.haslayer(IP):  # pachete cu strat IP
            ip_layer = packet[IP]

            # porturi default, inlocuite de cele actuale
            src_port = 0
            dst_port = 0

            is_malign = 0

            if packet.haslayer(UDP):
                udp_layer = packet[UDP]
                src_port = udp_layer.sport
                dst_port = udp_layer.dport
                                # dns, ntp, ssdp
                if dst_port in [53, 123, 1900]:
                    is_malign = 1  # malign

          # restul de pachete TCP
            elif packet.haslayer(TCP):
                tcp_layer = packet[TCP]
                src_port = tcp_layer.sport
                dst_port = tcp_layer.dport

            features = {
                'src_ip': int(ip_layer.src.replace('.', '')),  # ip to int
                'dst_ip': int(ip_layer.dst.replace('.', '')),  # ip to int
                'src_port': src_port,
                'dst_port': dst_port,
                'packet_size': len(packet),
                'ttl': ip_layer.ttl,
                'label': is_malign  # malign label
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

