from scapy.all import rdpcap, wrpcap
import random

#extragere block-uri
def extract_blocks(packets, block_size, num_blocks):
    total_packets = len(packets)
    blocks = []

    for _ in range(num_blocks):
        if total_packets <= block_size:
            blocks.append(packets)
        else:
            start = random.randint(0, total_packets - block_size)
            blocks.append(packets[start:start + block_size])

    return blocks

# amestecarea traficului normal cu malign, pe blocuri
def mix_traffic(clean_pcap_func, attack_pcap, output_pcap, block_size=50, num_blocks=100):
    clean_packets = rdpcap(clean_pcap_func)
    attack_packets = rdpcap(attack_pcap)

    clean_blocks = extract_blocks(clean_packets, block_size, num_blocks)
    attack_blocks = extract_blocks(attack_packets, block_size, num_blocks)

    mixed_packets = []
    for clean, attack in zip(clean_blocks, attack_blocks):
        mixed_packets.extend(clean + attack)

    # nr maxim de pachete, in total
    mixed_packets = mixed_packets[:5000]

    wrpcap(output_pcap, mixed_packets)
    print("Mixed pcap saved as {output_pcap} with {len(mixed_packets)} packets")

# amestecarea traficului normal cu malign, uniform
def mix_traffic_uniform(clean_pcap, attack_pcap, output_pcap):
    clean_packets = rdpcap(clean_pcap)
    attack_packets = rdpcap(attack_pcap)

    mixed_packets = clean_packets + attack_packets
    random.shuffle(mixed_packets)
    mixed_packets = mixed_packets[:5000] # limita pachete extrase

    wrpcap(output_pcap, mixed_packets)
    print("mixed pcap saved : {output_pcap} with {len(mixed_packets)} packets")


# fisierele
clean_pcap = "victim_clean_5k.pcap"  # fara ddos
syn_pcap = "victim_tcp_5k.pcap"
udp_pcap = "victim_udp_5k.pcap"
icmp_pcap = "victim_icmp_5k.pcap"

# export blocks
# mix_traffic(clean_pcap, syn_pcap, "clean_plus_syn_block.pcap")
# mix_traffic(clean_pcap, udp_pcap, "clean_plus_udp_block.pcap")
# mix_traffic(clean_pcap, icmp_pcap, "clean_plus_icmp_block.pcap")

# export uniform
mix_traffic_uniform(clean_pcap, syn_pcap, "clean_plus_syn_uniform.pcap")
mix_traffic_uniform(clean_pcap, udp_pcap, "clean_plus_udp_uniform.pcap")
mix_traffic_uniform(clean_pcap, icmp_pcap, "clean_plus_icmp_uniform.pcap")