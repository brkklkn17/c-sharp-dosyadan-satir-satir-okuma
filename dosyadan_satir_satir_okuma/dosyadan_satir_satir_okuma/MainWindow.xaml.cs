using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace dosyadan_satir_satir_okuma
{
	/// <summary>
	/// MainWindow.xaml etkileşim mantığı
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		//String sınıfımıza IEnumerator arayüzü ile iterasyon özelliği kazandırdık.
		//VeriYukle() fonksiyonunda da göreceğimiz üzere yapacağımız tek şey MoveNext() diyerek sonraki satıra geçmek. Current ile de bulunduğumuz satırı almak olacak.
		IEnumerator<String> _Satirlar = null;

		private void Button_Click(Object sender, RoutedEventArgs e)
		{
			//öncelikle butonumuzu pasif hale getirelim.
			button.IsEnabled = false;
			//test dosyasının yolunu uygulama ile aynı dizin olarak gösteriyoruz.
			String _DosyaAdi = AppDomain.CurrentDomain.BaseDirectory + "veri.txt";
			//test dosyamız yoksa oluşturalım varsa hiç bulaşmayalım.
			if (!File.Exists(_DosyaAdi))
			{
				//test dosyasındaki satır miktarı.
				Int32 _SatirSayisi = 1000000;
				//dosya yoksa yeni bir tane oluşturuyoruz varsa üzerine yazıyoruz.
				FileStream _Dosya = new FileStream(_DosyaAdi, FileMode.OpenOrCreate);
				//döngü ile veri satır sayısı kadar satır oluşturacağız.
				for (Int32 _Index = 0; _Index < _SatirSayisi; _Index++)
				{
					//veriyi oluşturuyoruz ve sonuna yeni satır karakteri ekliyoruz.
					String _SatirString = "test verisi: " + _Index + Environment.NewLine;
					//dosyaya yazacağımız için veriyi byte dizisine çeviriyoruz.
					Byte[] _SatirByte = Encoding.ASCII.GetBytes(_SatirString);
					//satırımızı dosyaya yazıyoruz.
					_Dosya.Write(_SatirByte, 0, _SatirByte.Length);
					//aşağıdaki satırı aktifleştirirsek yazma işleminin tamamlanmasını yüzdelik olarak görebiliriz.
					//ancak bu seferde programımızda performans kaybı meydana gelecektir.
					//this.Title = "%" + ((Double)_Index / (Double)_SatirSayisi) * 100;
				}
				//yazma işlemimiz tamamlandığına göre dosyamızı kapatabiliriz.
				_Dosya.Close();
				//bittiğini anlamak içinde form başlığında bunu belirtiyoruz.
				this.Title = "test dosyası oluşturuldu.";
			}
			//yazma işlemimi tamamlandığına göre dosyayı okuyabiliriz.
			//dosyadan veri okumak için bir çok farklı yöntem kullanabiliriz. ama bu programın mantığı listbox içinde scrollbarı en aşağı çektiğimizde yeni verilerin yüklenmesi olduğu için aşağıki yöntem hız açısından işimizi görecektir. örnek olarak File.ReadAllLines() fonksiyonu ile de okuma yapabilirdik veya FileStream sınıfını kullanarak da yapabilirdik ancak program tüm veriyi tek seferde okuyacağından işlem uzun süreceğinden ve yukarıda bahsettiğim mantığa aykırı olacağı için String sınıfına iterasyon özelliği kazandırdık(yukarıda tanımladığımız IEnumerator<String> _Satirlar kısmı)
			_Satirlar = File.ReadLines(_DosyaAdi).GetEnumerator();
			//okuma işlemimizi yaptıktan sonra verimizi listboxa ekleyebiliriz.
			//bunun için VeriYukle() fonksiyonunu listboxa eklemek istediğimiz yerlerde çağıracağız.
			VeriYukle();
		}

		private void VeriYukle()
		{
			//fonksiyonun her çağırılışında kaç satır okuyacağımızı belirliyoruz.
			Int32 _OkunacakSatir = 50;
			//okuma işlemini while döngüsü ile yapacağımız için kaç satır okuduğumuzu bilmemiz gerek.
			Int32 _OkunanSatir = 0;
			//döngünün bitip bitmemesi gerektiğini kontrol ettiğimiz koşulumuz.
			//eğer sınıra ulaşmış isek döngüden çıkacağız.
			while (_OkunanSatir < _OkunacakSatir && _Satirlar.MoveNext())
			{
				//Current ile şuan hangi satırda bulunuyorsak o satırı listboxa ekliyoru.
				listBox.Items.Add(_Satirlar.Current);
				//okunan satır sayısını 1 arttırdık. aksi halde döngümüz sonsuza kadar devam ederdi.
				_OkunanSatir++;
			}
			//pencere başlığında kaç satır okuduğumuzu görelim.
			this.Title = "okunan satır sayısı: " + listBox.Items.Count;
		}

		private void ScrollViewer_ScrollChanged(Object sender, ScrollChangedEventArgs e)
		{
			//scrollbarın en aşağıda olup olmadığını kontrol edeceğimiz için scrollviewer nesnesi oluşturduk
			//ve sender nesnesini scrollviewer türünde olduğunu bildirdik.
			ScrollViewer _ScrollBar = sender as ScrollViewer;
			//aşağıki koşul ile scrollbarın en aşağıda olup olmadığını kontrol ediyoruz.
			//koşulun doğruluğu 3 duruma bağlı. Bunlar;
			//ExtentHeightChange: scrollbarın kaydırılıp kaydırılmadığı
			//örnek olarak bu program bu değer ilk etapta 50 ilken scrollbar kaydırıldığında 0 oluyor.
			//VerticalOffset    : scrollbarın kaydırıldığında listbox içinde gözüken ilk satırın satır sayısı
			//örnek olarak scrollbar bir miktar kaydırıldında listboxın ilk elemanı 5. satır ise VerticalOffset
			//5 değerini alır. yani kaydırılan satır sayısı + 1
			//ScrollableHeight  : scrollbar aşağı ya da yukarı kaydırıldığında listbox dışında kalan satır sayısı
			//örnek olarak ilk veriyi yüklediğimizde ekranda 20 satır görüyoruz. 50 satır yüklediğimiz için
			//henüz göremediğimiz satır sayısı 30 oluyor. scrollbarı 1 satır aşağı kaydırdığımızda aşağıda
			//göremezdiğimiz 29 satır kalmış oluyor. yukarıda ise göremediğimiz 1 satırda var. toplamda 
			//yine göremediğimiz satır sayısı 30 olmuş oluyor.
			//mantık olarak inceleyecek olursak eğer scrollbar kaydırılmış ise(ExtentHeightChange) ve ilk satır
			//numarası(VerticalOffset) ile aşağı ve yukarı kaydırılabilecek satır sayısı(ScrollableHeight) eşit
			//ise scrollbarımız en aşağıda demektir.
			Boolean _ScrollBarEnAsagida = e.ExtentHeightChange == 0 && e.VerticalOffset == _ScrollBar.ScrollableHeight;
			//bu yüzden hemen bir miktar daha veri yüklüyoruz.
			if (_ScrollBarEnAsagida)
			{
				//okumaya başlamadan önce listboxdaki satır sayısını alıyoruz.
				Int32 _SonSatir = listBox.Items.Count - 1;
				//50 adet verimizi listboxa ekliyoruz.
				VeriYukle();
				//seçilen satır indisini yükleme yapılmadan önceki satır sayısına eşitliyoruz.
				listBox.SelectedIndex = _SonSatir;
				//ardından seçilen scrollbarı seçilen satıra götürüyoruz.
				listBox.ScrollIntoView(listBox.SelectedItem);
			}
		}

		private void Window_Loaded(Object sender, RoutedEventArgs e)
		{
			//normalde listboxın olayları arasında scrollchanged adında bir olay yok
			//ancak scrollviewer nesnesinde var. bu yüzden scrollviewer nesnesine erişip
			//scrollchanged özelliği tektiklendiğinde çağırılacak fonksiyonu ayarlıyoruz.
			//wpf üzerinde bir listboxa sağ tıklayıp şablonu düzenle dersek listboxı
			//3 alt öğeye ayırmış oluruz.
			//bunlardan ilki border yani aşağıda tanımladığımız.
			Decorator _Border = VisualTreeHelper.GetChild(listBox, 0) as Decorator;
			//borderın alt öğesi ise scrollbar.
			ScrollViewer _ScrollBar = _Border.Child as ScrollViewer;
			//son olarak da scrollchanged olayını fonksiyonumuza bağladık.
			_ScrollBar.ScrollChanged += ScrollViewer_ScrollChanged;
		}
	}
}
