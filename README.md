# MovieCutRecord
このツールは、[Beat Saber HTTP Status](https://github.com/opl-/beatsaber-http-status) のWebSocket送信機能を利用し、Beat Saberのプレイ情報をデータベースに記録します。拙作のBeat Saber プレイ動画カットツール[BS Movie Cut](https://github.com/rynan4818/bs-movie-cut)で使用するために製作しました。

**注意：現在作者はプレイ情報の記録に[DataRecorder](https://github.com/rynan4818/DataRecorder)を使用しています。機能が劣るため、特に理由がなければ本ツールは使用しないで下さい。**

# インストール方法

1. [Beat Saber HTTP Status](https://github.com/opl-/beatsaber-http-status)のインストール手順によって、HTTP Statusをインストールします。そして、HTTP Statusが正しく動作するか一度確認して下さい。

2. [リリースページ](https://github.com/rynan4818/movie-cut-record/releases)から最新のリリースをダウンロードします。

3. zipを適当なフォルダに解凍します。 例： C:\TOOL\MovieCutRecord\
**注意　Program Files や Program Files (x86) 下にコピーしないで下さい。**

4. MovieCutRecord.exe へのショートカットを作成してデスクトップに置くか、スタートメニューにピン留め等して起動できるようにして下さい。

# 使用方法

BeatSaberで遊ぶ時に起動しておくと、プレイ記録を自動作成します。何らかの障害で通信が切断された場合は、自動再接続を繰り返します。
BeatSaberとの起動順序の制限はありません、どちらを先に起動してもＯＫです。ツールの終了は、`END` と打ち込んで `Enter` して下さい。

**※注意  beatsaber.db (プレイ情報記録データベース)は MovieCutRecordをインストールしたフォルダに作成されます。[Beat Saber HTTP Status +Database](https://github.com/rynan4818/beatsaber-http-status-db) で記録済みの beatsaber.db に追記する場合は、MovieCutRecordの起動前にコピーして下さい。**

# 開発者向け

Visual Studio 2019 Community のNuGetパッケージマネージャで以下のライブラリを使用しています。
各ライブラリの開発者及びライセンスは以下の通りです。

- System.Data.SQLite.Core
  - https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki
  - 開発者：SQLite Development Team
  - ライセンス：パブリックドメイン

- Newtonsoft.Json
  - https://www.newtonsoft.com/json
  - 開発者：James Newton-King
  - ライセンス：MIT licence

- SuperSocket.ClientEngine.Core
  - http://www.supersocket.net/
  - 開発者：Kerry Jiang
  - ライセンス：APACHE LICENSE v2.0

- WebSocket4Net
  - https://archive.codeplex.com/?p=websocket4net
  - 開発者：Kerry Jiang
  - ライセンス：APACHE LICENSE v2.0

- System.*
  - 開発者：Microsoft
  - ライセンス：https://dotnet.microsoft.com/en/dotnet_library_license.htm
