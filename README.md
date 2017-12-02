# TechSummit 2017 Japan : Hands on Learning
## HOL002 & HOL006 - IoT × Bot の可能性：会議室空き確認 Bot 開発

Web カメラから撮影した画像から会議室の空きや店舗の混雑状況を判定、自動応答 Chat でその状況を確認 ～ そんなソリューションを
簡単に作成できる時代になりました。
Microsoft Cognitive Services の Custom Vision Service を利用した画像分析、および Microsoft Bot Framework を活用した
自動応答 Chatbot の開発を体験できるハンズオン トレーニングです。

## 内容
**Microsoft Cognitive Services を活用した 会議室空き確認 Bot 開発**

このハンズオンでは、まず Custom Vision Service を利用して、画像から会議室の空き状況を判 別するカスタム画像分析モデルを作成します。
次に、Web を経由して定期的に取得される 画像が Azure Blob ストレージに送られるという状況を踏まえ、画像がアップロードされるたびに 
Custom Vision Service の API により、自動で画像を分析して結果を Azure テーブルストレージに保存する仕組みを Azure Logic App を
利用して構築します。
そして、ユーザーインターフェースとして Microsoft Bot Framework を利用したボット (チ ャットボット(Chatbot)) により、会議室の空き状況を
自動で返答するボットを構築します。

![](https://github.com/ayako/TS17-CognitiveCVHoL/blob/master/media/1-12.PNG)
![](https://github.com/ayako/TS17-CognitiveCVHoL/blob/master/media/2-32.PNG)
![](https://github.com/ayako/TS17-CognitiveCVHoL/blob/master/media/3-19.PNG)

## 準備事項
このハンズオン ラボを行うには、以下が必要です。

- マイクロソフトアカウント
- Azure サブスクリプション
- Visual Studio 2017 Community エディションまたはこれ以降
- [Azure Storage Explorer](https://azure.microsoft.com/ja-jp/features/storage-explorer/)
- [Bot Framework C# Template](http://aka.ms/bf-bc-vstemplate)
- [Bot Channel Emulator](https://emulator.botframework.com/)
