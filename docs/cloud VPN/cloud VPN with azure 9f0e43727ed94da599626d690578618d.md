# cloud VPN with azure

## ref

[在 Google Cloud 和 Azure 之间创建高可用性 VPN 连接](https://cloud.google.com/network-connectivity/docs/vpn/tutorials/create-ha-vpn-connections-google-cloud-azure?hl=zh-cn)

## Concept

- 在 GCP 把 azure 當地端建立 cloud VPN
- 在 Azure 把 GCP 當地端建立 VPN gateway
- 建置流程
    - azure 建置初始資源
    - gcp 建置初始資源 + 創建 VPN
    - azure 創建 VPN
- point for init resource ⇒ ip should not overlap
- BGP 的概念是在指定的 VPN 網段中 ip 如何 mapping, 因為 azure 在 `169.254.21.*` and `169.254.22.*` 中間設定 VPN, 所以 GCP 要在他們旁邊設置 ip
    - ex: GCP:`169.254.21.5` ⇒ AZURE: `169.254.21.6`
    - ex: GCP:`169.254.21.9` ⇒ AZURE: `169.254.21.10`

## Step

### azure init resource

- 創建 **virtual network**
- 創建 **virtual network gateway (VPN mode)**
    - ***AZURE_ASN : `vpn` 識別號用預設即可(ex: 65515)***
    - ***AZURE_BGP_IP_0 : ex:*** `169.254.21.6`
    - ***AZURE_BGP_IP_1 : ex:*** `169.254.21.10`
    - configure BGP
- 記下得到的兩個外部 IP 是為以下代號
    - AZURE_GW_IP_0
    - AZURE_GW_IP_1

### gcloud

- create VPC
- create VPC subnet
    - subnet 建議設到 **`10.1.1.0/24`** 避免和 azure overlap
- 進入 **Network Connectivity** 功能
    - create **Cloud Routers**
        - set Google_ASN (ex: 65534)
    - VPN use “VPN setup wizard”
        - HA VPN
            - **Create Cloud HA VPN gateway**
                - 導出 2 個外部 IP
            - create peer VPN Gateway
                - AZURE_GW_IP_0: azure init resource 後得到的
                - AZURE_GW_IP_1: azure init resource 後得到的
            - Add VPN tunnels * 2 (set on prem)
                - use IKEv2
                - 設置密碼 PSK 並記下
            - set BGP session * 2
                - ASN is Azure ASN (65515)
                - manually BGP
                    - ex: GCP:`169.254.21.5` ⇒ AZURE: `169.254.21.6`
                    - ex: GCP:`169.254.21.9` ⇒ AZURE: `169.254.21.10`

![Untitled](cloud%20VPN%20with%20azure%209f0e43727ed94da599626d690578618d/Untitled.png)

### azure set VPN

- Local network gateway * 2
    - IP Address : Cloud VPN gateway(IP)
    - ASN : GCP ASN ex: 65534
    - configure BGP setting
    - BGP Peer IP : GCP cloud router BGP IP address
- [Virtual network gateway].[connection] * 2
    - site to site
    - enable BGP
    - PSK (GCP 中 add VPN tunnel 時創建的)
    - IKEv2

## verify

in GCP check this page

![Untitled](cloud%20VPN%20with%20azure%209f0e43727ed94da599626d690578618d/Untitled%201.png)

最少一條 tunnel 可用

## usage

目的, 允許 azure access gcp

- 在 GCP 一開始創建的子網 ex 10.1.1.0/24 上建置 VM
- 開啟防火牆允許 azure 中創建子網的流量進入 ex: default 10.0.0.0/24 中建制的 VM
- 注意, GCP 的 VPC 子網 和 azure 的 VNET 不可以網域 overlap , 此時因為兩個系統已經藉由 VPN 連結, 所以 azure 中 default 的 VM 可以 ping 到 GCP 中 10.1.1.0 的 VM

## Final

架構

VPC  ⇒ HA VPN ⇒ tunnel

VNET ⇒ VNET Gateway (VPN mode) ⇒ connection

GCP

- Cloud Router
    - 自己的 ASN (GCP)
- HA VPN 內含
    - HA VPN Gateway ⇒ 提供 VPN 所需外部 IP
    - peer VPN Gateway ⇒ Azure VPN 的外部 IP
    - tunnel ⇒ 設置協議和密碼
    - BGP session ⇒ Azure VPN 識別號和雙邊 VPN IP 轉換點。

Azure

- virtual network gateway (VPN mode)
    - 自己的 ASN (Azure)
    - 自己的 BGP 映射目標（GCP 中 BGP 轉換目標）
    - 提供 VPN 所需外部 IP
    - connection
        - 協議
        - 密碼 PSK
- Local network gateway
    - GCP VPN 的外部 IP
    - GCP 中 BGP 轉換來源

可以發現兩邊 VPN 概念相同, 但拆分邏輯差異很大。

### note: cloud run 連接 azure sql server (mssql)

GCP: 

啟動 server less VPC connecter 於 VPN 內網, 且掛在 cloud run 上, 選取 private ip route to VPC

Azure: 

在 sql 啟用 private access to VNet endPoint(具備內網 IP), VNet 於 VPN 內網

![截圖 2023-12-18 下午4.05.54.png](cloud%20VPN%20with%20azure%209f0e43727ed94da599626d690578618d/%25E6%2588%25AA%25E5%259C%2596_2023-12-18_%25E4%25B8%258B%25E5%258D%25884.05.54.png)

建立之後可以在私人端點確認 DNS內容，該 IP 即為 SQL Server 的 VNET IP。

![截圖 2023-12-18 下午4.08.18.png](cloud%20VPN%20with%20azure%209f0e43727ed94da599626d690578618d/%25E6%2588%25AA%25E5%259C%2596_2023-12-18_%25E4%25B8%258B%25E5%258D%25884.08.18.png)

可以在 VNET 建立 VM 去確認連線是否正常。

連線部分有三點要注意。

1. Connection String 要加上 TrustServerCertificate=True。
2. Server 欄位直接使用 VNET IP 位址。
3. 使用者帳號要加上 Server Name。例：db_user@mealorder

Container: 

用 endPoint IP 連線 database 且帳號要用 AD 全名(xxx@server)