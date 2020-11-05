READ ME.txt 
#Project Information at notion [https://www.notion.so/GUI-Programming-Project-5871d5806f0e46ee9903c990f9d169c5]

Quá trình suy nghĩ và đưa ra giải pháp

Sử dụng richtextbox thay cho textbox vì textbox kjhoong làm được syntax highlighting 

Bước đầu ta có 1 xaml TabControl => Ngay tại cửa sổ tạo Window sẽ thêm sẵn 1 tabItem có header là Document 1* với * là chưa lưu 

+ Tính năng New File 

=> Với mỗi click New File command (Ctrl + N) ta sẽ sử dụng hàm 		InitTab để tạo tab mới đồng thời add tab đó vào List tabItems, 	thêm biến iSaved=false cho tab mới, thêm file Path="", thêm tab 	mới này vào tabControl

+ Với tính năng open File 

=> ta sử dụng OpenFileDialog để mở file 
Nếu người dùng chọn file và nhấp open => Ta sẽ xét 2 TH 
	TH1: File chưa có gì thì sẽ mở ngay tại tab đó 
	TH2: File đã có text thì mở ở tab mới 
Sau đó gán thuộc tính isSave của tabControl.SelectedIndex = true vì file này đã save. Scroll to end và thêm filePath vào 

+ Với tính năng save 

*Để thực hiện tính năng save này ta sẽ thực hiện tính năng save as trước


Bước 1: Tạo tính năng isSaved "*" sẽ tự động thêm dấu * nếu file đã được chỉnh sửa và chưa lưu. Nếu file đã lưu thì sẽ tự động xóa dấu * đó và cập nhật mới vào file

Bước 2: Ta sẽ kiểm tra là file này đã save chưa bằng cách kiểm tra thuộc tính isSaved nếu đã save sẽ bỏ qua bước 3 
 
Bước 3: Ta sẽ có 2 TH chính 

	-File chưa save As lần nào ( nghĩa là FilePath Rỗng )
	Ta sẽ gọi hàm Save As như thường
	-File đã được save As hoặc được mở từ một vị trí bất kỳ  
	Ta sẽ ghi đè text này lên file ở vị trí filePath đã mở 
	Sau đó sẽ xóa dấu * ở header và set isSaved=true

+ Với tính năng save as 
tạo obj SaveFileDialog và thêm file vào 
đồng thời set isSaved=true và filePath là đường dẫn của file 

+ Với tính năng save all 
Ta sẽ thực hiện một vòng lặp hết tất cả các isSaved 
nếu phát hiện ra mục nào chưa save thì sẽ gọi hàm save cho tabItem đó

+ Với chức năng Exit và Close Window 
	Chức năng exit sẽ cấu tạo từ những hàm close tab (remove tab) 
	Remove tab nghĩa là sẽ tắt tab đó đi chứ không có nghĩa là xóa nó  
	Chức năng remove sẽ remove với tab đã save và suggest save cho 	
	những tab chưa save 

+Với chức năng mở terminal ngay tại path của file hoặc mở terminal bình thường ta sử dụng Process.Start() 
	Với file ta sử dụng thêm directory Parent để truy cập vào thư mục cha có chứa file đó 

Có các hàm phụ như 
	TextBox_TextChanged() Nếu có sự thay đổi ở RichTextBox lập tức gọi hàm AddSavedIcon (luôn luôn có dấu * khi thay đổi)
	RemoveSavedIcon() bỏ dấu * ở header và chỉnh isSaved=true
	AddSavedIcon() thêm dấu * ở header và chỉnh isSaved=false
	SetText(RichTextBox,string) dùng để thêm content vào RichTextBox
	GetText(RichTextBox) dùng để lấy content của RichTextBox nếu cần
