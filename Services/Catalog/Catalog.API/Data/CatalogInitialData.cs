using Marten.Schema;

namespace Catalog.API.Data;

public class CatalogInitialData : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        using var session = store.LightweightSession();

        if (await session.Query<Product>().AnyAsync())
            return;

        // Marten UPSERT will cater for existing records
        session.Store<Product>(GetPreconfiguredProducts());
        await session.SaveChangesAsync();
    }

    private static IEnumerable<Product> GetPreconfiguredProducts() => new List<Product>()
            {
                // Kutu Oyunları
                new Product()
                {
                    Id = Guid.Parse("5334c996-8457-4cf0-815c-ed2b77c4ff61"),
                    Name = "Squid Game 5 Taş Oyunu",
                    Description = "Squid Game 5 Taş Oyunu, Toyzzshop koleksiyonunun bir parçasıdır. Popüler dizinin oyuncuyla buluşan hali!",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/667d6_Squid_Game_5_Tas_Oyunu_.jpg",
                    Price = 149.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },
                new Product()
                {
                    Id = Guid.Parse("c67d6323-e8b0-4bdd-a7e6-a593eb6068e8"),
                    Name = "Smile Games Günün Sorusu Kutu Oyunu",
                    Description = "Günlük sorularla eğlenceli zeka oyunu. Aile boyu keyifli vakit geçirmek için ideal.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/e802c_Smile_Games_Gunun_Sorusu_Kutu_Oyunu.jpg",
                    Price = 274.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },
                new Product()
                {
                    Id = Guid.Parse("4f136e07-cc90-4d8b-b847-68c0a3331d79"),
                    Name = "Smile Games Matematik Oyunu",
                    Description = "Matematik becerilerini geliştiren eğitici kutu oyunu. Çocuklar için sayılarla eğlence.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/e8c20_Smile_Games_Matematik_Oyunu.jpg",
                    Price = 589.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },
                new Product()
                {
                    Id = Guid.Parse("6ec1297b-ec0a-4aa1-be25-6726e3b51a27"),
                    Name = "UNO Reverse Pack Eklenti Paketi",
                    Description = "UNO Reverse Pack eklenti paketi ile oyuna yeni kurallar ve heyecan ekleyin!",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/0ebd5_UNO_Eklenti_Paketleri_JCV55.jpg",
                    Price = 119.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },
                new Product()
                {
                    Id = Guid.Parse("b786103d-c621-4f5a-b498-23312a3fa792"),
                    Name = "UNO Stack Pack Eklenti Paketi",
                    Description = "UNO Stack Pack ile kartları biriktirme stratejisi oyuna yeni boyut katıyor.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/3f171_UNO_Eklenti_Paketleri_JCV55.jpg",
                    Price = 119.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },
                new Product()
                {
                    Id = Guid.Parse("c4bbc4a2-4555-45d8-97cc-2a99b2167bff"),
                    Name = "UNO Teams Kartlar Kart Oyunu",
                    Description = "UNO Teams ile takım halinde strateji geliştirin ve eğlenceli anlar yaşayın.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/8edb7_UNO_Teams_Kartlar_Kart_Oyunu_HXT58.jpg",
                    Price = 319.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },
                new Product()
                {
                    Id = Guid.Parse("93170c85-7795-489c-8e8f-7dcf3b4f4188"),
                    Name = "Smile Games Sudoku Zeka Oyunu",
                    Description = "Klasik Sudoku oyunu ile zeka gelişimi. Her yaş için uygun seviyeler.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/4a165_Smile_Games_Sudoku_RD5284.jpg",
                    Price = 399.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },
                new Product()
                {
                    Id = Guid.Parse("f5a9d6b2-8c3e-4f1a-9b7d-2e8f6c4a5b3c"),
                    Name = "Duel Kutu Oyunu",
                    Description = "İki kişilik strateji oyunu. Düşünce gücünüzü test edin ve rakibinizi alt edin.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/779e6_Duel_Kutu_Oyunu.jpg",
                    Price = 374.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },
                new Product()
                {
                    Id = Guid.Parse("7a8b9c2d-3e4f-5a6b-7c8d-9e0f1a2b3c4d"),
                    Name = "Smile Games Domino Oyunu",
                    Description = "Geleneksel domino oyununun modern versiyonu. Ailece keyifli saatler.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/b9aae_Smile_Games_Domino_Oyunu.jpg",
                    Price = 539.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },
                new Product()
                {
                    Id = Guid.Parse("8b7c6d5e-4f3a-2b1c-9d8e-7f6a5b4c3d2e"),
                    Name = "Rubik's Zeka Küpü 3x3",
                    Description = "Dünyaca ünlü Rubik's Küpü. Zeka ve sabır geliştiren efsane bulmaca.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/1c7db_Rubik_s_Zeka_Kupu_3x3.jpg",
                    Price = 469.99M,
                    Category = new List<string> { "Kutu Oyunları" }
                },

                // Müzik Aletleri
                new Product()
                {
                    Id = Guid.Parse("9c8d7e6f-5a4b-3c2d-1e0f-9a8b7c6d5e4f"),
                    Name = "Bontempi Işıklı Mikrofonlu Elektronik Tabureli Org",
                    Description = "Işıklı ve mikrofonlu elektronik org. Çocuklar için mükemmel müzik deneyimi.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/1de79_Bontempi_Isikli_Mikrofonlu_Elektronik_Tabureli_Org.jpg",
                    Price = 2699.00M,
                    Category = new List<string> { "Müzik Aletleri" }
                },
                new Product()
                {
                    Id = Guid.Parse("ad9e8f7a-6b5c-4d3e-2f1a-0b9c8d7e6f5a"),
                    Name = "Bontempi Ayaklı Mikrofonlu Pembe Elektronik Çocuk Orgu",
                    Description = "Pembe renkli ayaklı mikrofonlu elektronik org. Küçük müzisyenler için ideal.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/31527_Bontempi_Ayakli_Mikrofonlu_Pembe_Elektronik_Cocuk_.jpg",
                    Price = 1429.00M,
                    Category = new List<string> { "Müzik Aletleri" }
                },
                new Product()
                {
                    Id = Guid.Parse("be0f9a8b-7c6d-5e4f-3a2b-1c0d9e8f7a6b"),
                    Name = "Akustik Çocuk Gitarı 56 cm (Krem)",
                    Description = "Çocuklar için özel olarak tasarlanmış akustik gitar. Gerçek müzik deneyimi.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/a37b6_Akustik_Cocuk_Gitari_56_cm.jpg",
                    Price = 449.99M,
                    Category = new List<string> { "Müzik Aletleri" }
                },
                new Product()
                {
                    Id = Guid.Parse("cf1a0b9c-8d7e-6f5a-4b3c-2d1e0f9a8b7c"),
                    Name = "Drum Rock Elektronik Bateri Seti",
                    Description = "Profesyonel elektronik bateri seti. Gerçek davul deneyimi yaşayın.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/2cbf6_Drum_Rock_Elektronik_Bateri_Seti.jpg",
                    Price = 2529.00M,
                    Category = new List<string> { "Müzik Aletleri" }
                },
                new Product()
                {
                    Id = Guid.Parse("d02b1c0d-9e8f-7a6b-5c4d-3e2f1a0b9c8d"),
                    Name = "Hayvan Desenli Ukulele 55 cm (Zürafa Desenli)",
                    Description = "Renkli hayvan desenli ukulele. Çocuklar için eğlenceli müzik başlangıcı.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/72019_Hayvan_Desenli_Ukulele_55_cm.jpg",
                    Price = 459.99M,
                    Category = new List<string> { "Müzik Aletleri" }
                },
                new Product()
                {
                    Id = Guid.Parse("e13c2d1e-0f9a-8b7c-6d5e-4f3a2b1c0d9e"),
                    Name = "Çocuk Müzik Aleti Seti",
                    Description = "Çeşitli müzik aletlerini içeren komplet set. Müziğe ilk adım.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/520df_Cocuk_Muzik_Aleti_Seti_.jpg",
                    Price = 659.00M,
                    Category = new List<string> { "Müzik Aletleri" }
                },

                // Eğitici Oyuncaklar
                new Product()
                {
                    Id = Guid.Parse("f24d3e2f-1a0b-9c8d-7e6f-5a4b3c2d1e0f"),
                    Name = "Fisher Price Matematikçi Timsah",
                    Description = "Matematik öğrenmeyi eğlenceli hale getiren interaktif timsah oyuncak.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/da4da_Fisher_Price_Matematikci_Timsah_JCT13.jpg",
                    Price = 1399.99M,
                    Category = new List<string> { "Eğitici Oyuncaklar" }
                },
                new Product()
                {
                    Id = Guid.Parse("035e4f3a-2b1c-0d9e-8f7a-6b5c4d3e2f1a"),
                    Name = "Marvel Spiderman İngilizce Türkçe Laptop",
                    Description = "Spiderman temalı eğitici laptop. İngilizce ve Türkçe öğrenme.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/efc6a_Marvel_Spiderman_Ingilizce_Turkce_Laptop.jpg",
                    Price = 2799.99M,
                    Category = new List<string> { "Eğitici Oyuncaklar" }
                },
                new Product()
                {
                    Id = Guid.Parse("146f5a4b-3c2d-1e0f-9a8b-7c6d5e4f3a2b"),
                    Name = "Manyetik Uzay Roketi 55 Parça",
                    Description = "Manyetik parçalarla uzay roketi inşa etme seti. STEM öğrenme.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/f7c39_Manyetik_Uzay_Roketi_55_Parca.jpg",
                    Price = 799.99M,
                    Category = new List<string> { "Eğitici Oyuncaklar" }
                },

                // Peluş Oyuncaklar
                new Product()
                {
                    Id = Guid.Parse("2570a6b5-4c3d-2e1f-0a9b-8c7d6e5f4a3b"),
                    Name = "Sesli Disney Stitch Real Fx Elektronik Kukla 30 cm",
                    Description = "Disney Stitch karakterinin sesli ve hareketli gerçekçi versiyonu.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/65d38_Sesli_Disney_Stitch_Real_Fx_Elektronik_Kukla_30_cm.jpg",
                    Price = 4289.99M,
                    Category = new List<string> { "Peluş Oyuncaklar" }
                },
                new Product()
                {
                    Id = Guid.Parse("3681b7c6-5d4e-3f2a-1b0c-9d8e7f6a5b4c"),
                    Name = "Sesli Inside Out 2 Mini Peluş (Neşe)",
                    Description = "Inside Out 2 filminin sevilen karakteri Neşe'nin sesli peluş versiyonu.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/cc6df_Sesli_Inside_Out_2_Mini_Pelus_NDN01000.jpg",
                    Price = 1199.99M,
                    Category = new List<string> { "Peluş Oyuncaklar" }
                },
                new Product()
                {
                    Id = Guid.Parse("4792c8d7-6e5f-4a3b-2c1d-0e9f8a7b6c5d"),
                    Name = "Baby Paws Yummy Chihuahua Peluş",
                    Description = "Sevimli Chihuahua köpek peluş oyuncak. Çok yumuşak ve sevimli.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/2eb2b_Baby_Paws_Yummy_Chihuahua_Pelus_BAW03100.jpg",
                    Price = 1199.99M,
                    Category = new List<string> { "Peluş Oyuncaklar" }
                },

                // Oyuncak Arabalar
                new Product()
                {
                    Id = Guid.Parse("58a3d9e8-7f6a-5b4c-3d2e-1f0a9b8c7d6e"),
                    Name = "1:64 Hot Wheels The Hot Ones Fiat 500 Topolino (1936)",
                    Description = "Klasik 1936 Fiat 500 Topolino model arabası. Koleksiyoncular için ideal.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/8f4a1_164_Hot_Wheels_The_Hot_Ones_Die_Cast_Tekli_Arabala.jpg",
                    Price = 249.99M,
                    Category = new List<string> { "Oyuncak Arabalar" }
                },
                new Product()
                {
                    Id = Guid.Parse("69b4eaf9-8a7b-6c5d-4e3f-2a1b0c9d8e7f"),
                    Name = "1:24 Lamborghini Sian FKP 37 Model Araba (Yeşil)",
                    Description = "Lamborghini'nin en yeni süper spor araba modelinin detaylı replikası.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/879a7_1_24_Lamborghini_Sian_FKP_37_Model_Araba.jpg",
                    Price = 1199.00M,
                    Category = new List<string> { "Oyuncak Arabalar" }
                },
                new Product()
                {
                    Id = Guid.Parse("7ac5fb0a-9b8c-7d6e-5f4a-3b2c1d0e9f8a"),
                    Name = "1:12 USB Şarjlı Uzaktan Kumandalı 4x4 Arazi Aracı",
                    Description = "Gerçekçi 4x4 arazi aracı. USB şarjlı ve uzaktan kumandalı.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/29bf8_1_12_USB_Sarjli_Uzaktan_Kumandali_4x4_Arazi_Araci.jpg",
                    Price = 3699.00M,
                    Category = new List<string> { "Oyuncak Arabalar" }
                },

                // Yapım Oyuncakları
                new Product()
                {
                    Id = Guid.Parse("8bd60c1b-ac9d-8e7f-6a5b-4c3d2e1f0a9b"),
                    Name = "LEGO Disney Walt Disney Hatırası Kamera",
                    Description = "Walt Disney'in hatırasına özel LEGO kamera seti. Nostaljik tasarım.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/ebcef_LEGO_Disney_Walt_Disney_Hatirasi_Kamera_43230_.jpg",
                    Price = 3699.00M,
                    Category = new List<string> { "Yapım Oyuncakları" }
                },
                new Product()
                {
                    Id = Guid.Parse("9ce71d2c-bd0e-9f8a-7b6c-5d4e3f2a1b0c"),
                    Name = "LEGO Marvel X-Men: X-Mansion",
                    Description = "X-Men'in ünlü X-Mansion binasının detaylı LEGO versiyonu.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/2abd5_LEGO_Marvel_XMen_X-Mansion_76294_.jpg",
                    Price = 12999.00M,
                    Category = new List<string> { "Yapım Oyuncakları" }
                },
                new Product()
                {
                    Id = Guid.Parse("adf82e3d-ce1f-0a9b-8c7d-6e5f4a3b2c1d"),
                    Name = "LEGO Technic Ferrari SF-24 F1 Araba",
                    Description = "Ferrari'nin 2024 F1 aracının teknik detaylarla dolu LEGO versiyonu.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/b1b4a_LEGO_Technic_Ferrari_SF_24_F1_Araba_42207_.jpg",
                    Price = 9399.00M,
                    Category = new List<string> { "Yapım Oyuncakları" }
                },

                // Oyuncak Bebekler ve Aksesuarları
                new Product()
                {
                    Id = Guid.Parse("be093f4e-df2a-1b0c-9d8e-7f6a5b4c3d2e"),
                    Name = "Barbie Dream Besties Paten Partisi Teresa Bebek",
                    Description = "Barbie'nin arkadaşı Teresa ile paten partisi seti. Aksesuarlarla birlikte.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/59611_Barbie_Dream_Besties_Paten_Partisi_Teresa_Bebek_ve.jpg",
                    Price = 1499.99M,
                    Category = new List<string> { "Oyuncak Bebekler ve Aksesuarları" }
                },
                new Product()
                {
                    Id = Guid.Parse("cf1a405f-ea3b-2c1d-0e9f-8a7b6c5d4e3f"),
                    Name = "Cry Babies Love Banyo Zamanı Dana Bebek 30 cm",
                    Description = "Banyo zamanı temalı ağlayan bebek. Su ile etkileşimli özellikler.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/55349_Cry_Babies_Love_Banyo_Zamani_Dana_Bebek_30_cm_CYB6.jpg",
                    Price = 1799.99M,
                    Category = new List<string> { "Oyuncak Bebekler ve Aksesuarları" }
                },
                new Product()
                {
                    Id = Guid.Parse("d02b516a-fb4c-3d2e-1f0a-9b8c7d6e5f4a"),
                    Name = "Disney Karlar Ülkesi Şarkı Söyleyen Elsa Bebek",
                    Description = "Sesli Disney Elsa bebek. Karlar Ülkesi filminin şarkılarını söylüyor.",
                    ImageFile = "https://docsd.toyzzshop.com/product/300x300/abcc5_Sesli_Disney_Karlar_Ulkesi_Sarki_Soyleyen_Elsa_Beb.jpg",
                    Price = 1399.99M,
                    Category = new List<string> { "Oyuncak Bebekler ve Aksesuarları" }
                }
            };

}
