# Faiz Hesaplama Sistemi - Güncelleme

## Genel Bakış

Projeye hem yıllık hem de aylık faiz hesaplama özelliği eklenmiştir. Kullanıcılar artık faiz türünü seçebilir ve farklı hesaplama yöntemlerini karşılaştırabilir.

## Yeni Özellikler

### 1. Faiz Türü Seçimi
- **Yıllık Faiz (Annual)**: Geleneksel basit faiz hesaplama
- **Aylık Faiz (Monthly)**: Bileşik faiz hesaplama

### 2. Faiz Hesaplama Formülleri

#### Yıllık Faiz (Basit Faiz)
```
A = P(1 + rt)
```
- A: Toplam tutar
- P: Anapara
- r: Yıllık faiz oranı
- t: Zaman (yıl)

#### Aylık Faiz (Bileşik Faiz)
```
A = P(1 + r)^t
```
- A: Toplam tutar
- P: Anapara
- r: Aylık faiz oranı (yıllık oran / 12)
- t: Taksit sayısı (ay)

## API Endpoint'leri

### 1. Faiz Türlerini Listele
```
GET /api/Installment/interest-types
```

**Response:**
```json
[
  {
    "value": 1,
    "name": "Yıllık",
    "description": "Faiz oranı yıllık bazda uygulanır"
  },
  {
    "value": 2,
    "name": "Aylık",
    "description": "Faiz oranı aylık bazda uygulanır (bileşik faiz)"
  }
]
```

### 2. Taksit Hesaplama
```
POST /api/Installment/calculate
```

**Request Body:**
```json
{
  "originalAmount": 10000,
  "interestRate": 0.12,
  "interestType": 1,
  "totalInstallments": 12
}
```

**Response:**
```json
{
  "originalAmount": 10000,
  "interestRate": 0.12,
  "interestType": 1,
  "totalInstallments": 12,
  "totalAmount": 11200,
  "perInstallmentAmount": 933.33,
  "interestTypeDescription": "Yıllık"
}
```

### 3. Faiz Türleri Karşılaştırması
```
POST /api/Installment/calculate-comparison
```

**Request Body:**
```json
{
  "originalAmount": 10000,
  "interestRate": 0.12,
  "totalInstallments": 12
}
```

**Response:**
```json
{
  "originalAmount": 10000,
  "interestRate": 0.12,
  "totalInstallments": 12,
  "annualCalculation": { ... },
  "monthlyCalculation": { ... },
  "difference": 200,
  "differencePercentage": 1.79
}
```

### 4. Taksit Oluşturma
```
POST /api/Installment
```

**Request Body:**
```json
{
  "userId": 1,
  "originalAmount": 10000,
  "interestRate": 0.12,
  "interestType": 1,
  "totalInstallments": 12,
  "nextPaymentDate": "2024-02-01T00:00:00Z"
}
```

## Veritabanı Değişiklikleri

### Installment Tablosu
- `InterestType` alanı eklendi (int)
  - 1: Yıllık (Annual)
  - 2: Aylık (Monthly)

## Kullanım Örnekleri

### Örnek 1: 10.000 TL, %12 Yıllık Faiz, 12 Taksit
- **Yıllık Faiz**: Toplam = 11.200 TL, Taksit = 933.33 TL
- **Aylık Faiz**: Toplam = 11.268 TL, Taksit = 939.00 TL

### Örnek 2: 50.000 TL, %18 Yıllık Faiz, 24 Taksit
- **Yıllık Faiz**: Toplam = 68.000 TL, Taksit = 2.833.33 TL
- **Aylık Faiz**: Toplam = 71.629 TL, Taksit = 2.984.54 TL

## Test Etme

`test_installment_api.http` dosyasını kullanarak tüm endpoint'leri test edebilirsiniz. Bu dosya:
- Faiz türlerini listeler
- Farklı faiz türleriyle hesaplama yapar
- Faiz türlerini karşılaştırır
- Yeni taksit oluşturur

## Notlar

1. **Varsayılan Değer**: Yeni taksitler için varsayılan faiz türü "Yıllık"tır
2. **Geriye Uyumluluk**: Mevcut taksitler için InterestType = 1 (Yıllık) olarak ayarlanır
3. **Hesaplama Farkı**: Aylık faiz genellikle daha yüksek toplam tutar verir çünkü bileşik faiz uygulanır
4. **Validasyon**: Faiz oranı negatif olamaz, anapara ve taksit sayısı pozitif olmalıdır

## Gelecek Geliştirmeler

- Karma faiz türleri (karma yıllık/aylık)
- Erken ödeme indirimi
- Faiz oranı değişikliği takibi
- Detaylı ödeme planı görüntüleme
