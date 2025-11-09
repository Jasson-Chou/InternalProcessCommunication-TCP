# InternalProcessCommunicationWithTCP

## 簡介
本專案展示如何使用本地 TCP方式在同一台主機上的多個進程/執行個體之間進行通訊。專案包含一個輕量的 IPC 庫 `IPCLib`（含 `TCPServer`、`TCPClient`、AES 加密等）以及一個範例程式 `InternalProcessCommunicationWithTCPDemo`，用來測試與量測通訊效能與正確性。

## 專案結構
- `IPCLib`：核心函式庫，提供 `TCPServer`、`TCPClient` 與相關工具（如 `AesEncryption`、`IPCKeywords`）。
- `InternalProcessCommunicationWithTCPDemo`：示範應用，包含多種測試案例（Case1~Case5）用於不同通訊流程與效能量測。

## 執行需求
- .NET Framework4.6.1
- C#7.3

## 編譯與執行
1. 使用 Visual Studio 開啟解決方案 `InternalProcessCommunicationWithTCP.sln`。
2. 設定啟動專案為 `InternalProcessCommunicationWithTCPDemo`。
3. 建置並執行。

程式啟動後會建立一個 TCP Server（隨機可用 port），並啟動多個範例 clients。透過 Console介面輸入選項來執行不同測試案例。

## 範例測試說明
- Case1：Server 發送字串 -> Client讀取，單趟 RTT 測量。
- Case2：Client 發送字串 -> Server讀取，單趟 RTT 測量。
- Case3：Server -> Client -> Server（雙向回傳），測量完整回合時間。
- Case4：Client -> Server -> Client（雙向回傳），測量完整回合時間。
- Case5：多 clients 同時運作並行測試，可測試多連線情況下延遲與平均值。

每個案例會進行多次迭代（預設 `TestCount =4096`），並將每次迭代耗時記錄到 `TestDatas` 資料夾下的 CSV 檔案（預設檔名 `CaseN.csv`）。

## 使用介面要點
- 啟動後的 Console會顯示可選功能與設定：
 - 輸入 `1`~`5` 執行對應 Case。
 - 可設定 Delay（毫秒）輸入格式例如 `Delay10.5`，在每次迴圈間加上指定延遲以模擬不同壓力環境。
 - Case5 可指定並行 client 數量（範圍1 ~100，視設定 `maxClient`）。

## 輸出檔案
- 測試結果會輸出到啟動目錄下的 `TestDatas` 資料夾，檔案以 `Case1.csv`、`Case2.csv`… 命名，內含每次迭代的耗時（微秒）與總耗時統計。

## 注意事項與最佳實務
- 範例使用阻塞讀取/寫入與簡單同步（並在一些路徑使用 `Parallel.For`）來模擬通訊。實務上應考慮非同步 IO、例外處理、與連線重試機制以提升穩定性。
- `TestCount` 與 client 數量過高可能造成主機資源飽和，請視系統能力調整。
- 若要在不同主機間通訊，請確保防火牆與可用 port 配置允許對應連線。

## 延伸/修改建議
- 在 `IPCLib` 中擴充更完整的錯誤處理與日誌紀錄。
- 將同步測試改為非同步 `async/await` 流程以衡量真實非同步效能。
- 支援可配置的 port 與加密選項，並提供單元測試以驗證通訊正確性。
