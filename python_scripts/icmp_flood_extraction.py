from scapy.all import rdpcap, IP, ICMP, Ether
import pandas as pd
import sys

def extract_features_from_pcap(pcap_file):
    packets = rdpcap(pcap_file) # pachetele din fisierul original
    data = []

    for packet in packets:
        if packet.haslayer(IP):
            ip_layer = packet[IP]
            is_icmp_flood = 0

            if packet.haslayer(ICMP):
                icmp_layer = packet[ICMP]
                if icmp_layer.type == 8 and icmp_layer.code == 0:  # ping
                    is_icmp_flood = 1  # malign packet

            features = {
                'src_ip': int(ip_layer.src.replace('.', '')),
                'dst_ip': int(ip_layer.dst.replace('.', '')),
                'packet_size': len(packet),
                'ttl': ip_layer.ttl,
                'ip_flags': int(ip_layer.flags),  # ip flag
                'fragment_offset': ip_layer.frag,
                'ip_header_length': ip_layer.ihl * 4,
                'ip_checksum': ip_layer.chksum,
                'src_mac': int(packet.src.replace(':', ''), 16) if packet.haslayer(Ether) else 0,  # mac sub forma de int
                'dst_mac': int(packet.dst.replace(':', ''), 16) if packet.haslayer(Ether) else 0,
                'label': is_icmp_flood
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
