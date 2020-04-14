#include "pch.h"
#include "AccountLoginDlg.h"
#include "framework.h"
#include "resource.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

BEGIN_MESSAGE_MAP(AccountLoginDlg, CDialogEx)
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDOK, &AccountLoginDlg::OnBnClickedLogin)
	
END_MESSAGE_MAP()

AccountLoginDlg::AccountLoginDlg(CTankLoginPlusDlg* mainDlg,CWnd* pParent):
	CDialogEx(IDD_LOGIN, pParent)
	, inputID(_T(""))
	, inputPassword(_T(""))
{
	m_hIcon = AfxGetApp()->LoadIcon(IDI_ICON5);
	this->mainDlg = mainDlg;
}

void AccountLoginDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialogEx::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT1, inputID);
	DDX_Text(pDX, IDC_EDIT2, inputPassword);
}

BOOL AccountLoginDlg::OnInitDialog()
{
	CDialogEx::OnInitDialog();

	// ���ô˶Ի����ͼ�ꡣ  ��Ӧ�ó��������ڲ��ǶԻ���ʱ����ܽ��Զ�
	//  ִ�д˲���
	SetIcon(m_hIcon, TRUE);			// ���ô�ͼ��
	SetIcon(m_hIcon, FALSE);		// ����Сͼ��

	return TRUE;  // ���ǽ��������õ��ؼ������򷵻� TRUE
}

void AccountLoginDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // ���ڻ��Ƶ��豸������

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// ʹͼ���ڹ����������о���
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// ����ͼ��
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialogEx::OnPaint();
	}
}

HCURSOR AccountLoginDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}


void AccountLoginDlg::OnBnClickedLogin()
{
	// TODO: �ڴ���ӿؼ�֪ͨ����������
	//CDialogEx::OnOK();
	UpdateData(true);
	HttpHelper http;
	std::wstringstream ss;
	ss << L"param={'user_id':'";
	ss << this->inputID.GetBuffer();
	ss << L"','user_password':'";
	ss << this->inputPassword.GetBuffer();
	ss << L"'}";
	std::string param = HttpHelper::wstringTostring(ss.str());
	std::string resu=http.postData("www.bestxiaoxiang.top", "/Tank_Service_SSM/user_login",param);
	int pos = resu.find("\r\n\r\n", 0);
	std::string result = "";
	if (pos >= 0) result = resu.substr(pos+4, resu.size());
	if (result == "false"||result=="") {
		MessageBox(L"�˺Ż�������󣬵�¼ʧ�ܣ�", L"��¼ʧ��", 0);
		this->mainDlg->setLoginState(false);
	}
	else {
		std::vector<std::string> s = http.split_str(result, ",");
		std::string id = *(s.begin() + 1);
		std::string name = *(s.begin() + 2);
		this->mainDlg->setAccount(id, name);
		this->mainDlg->setLoginState(true);
		
		this->mainDlg->UpdateData(false);
		
		CDialogEx::OnOK();
	}
}

