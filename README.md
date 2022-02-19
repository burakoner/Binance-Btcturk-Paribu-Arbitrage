# Binance Btcturk Paribu Arbitrage
 
Binance-BtcTurk-Paribu arasında klasik ve çapraz arbitraj fırsatlarını arayıp belirlediğiniz kriterlere uygun fırsatları Telegram ile bildiren bir yazılım.

# Kurulum

000/BinanceParibu202202191258.zip dosyasındaki Sql Server yedeğini restore edin.
Arbitrage.CoreApi/AppConnections.cs dosyasında Sql Server bağlantı parametlerini düzenleyin.
TelegramListener/Program.cs dosyasında ve veritabanında APP_SETTINGS tablosunda Telegram Bot Api anahtarını girin.
Projeyi publish edip IIS üzerinde ayağa kaldırın. Bu kadar.

IIS üzerinde devamlı çalışması için 000/IIS/IIS Application Pool.png dosyasındaki ayarları yapmayı unutmayın.
Web Application Vue SPA olduğundan IIS üzerine 000/IIS/urlrewrite2.exe modülünü kurmanız gerekecek.

# Ekran Görüntüleri

![Screenshot-01](https://raw.githubusercontent.com/burakoner/Binance-Btcturk-Paribu-Arbitrage/main/000/Screenshot-01.jpeg)
![Screenshot-02](https://raw.githubusercontent.com/burakoner/Binance-Btcturk-Paribu-Arbitrage/main/000/Screenshot-02.jpeg)
![Screenshot-03](https://raw.githubusercontent.com/burakoner/Binance-Btcturk-Paribu-Arbitrage/main/000/Screenshot-03.jpeg)
![Screenshot-04](https://raw.githubusercontent.com/burakoner/Binance-Btcturk-Paribu-Arbitrage/main/000/Screenshot-04.jpeg)