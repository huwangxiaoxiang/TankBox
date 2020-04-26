#pragma once
#include <afxdialogex.h>
#include "TankLoginPlusDlg.h"
class AccountLoginDlg :
	public CDialogEx
{
	// ����
public:
	AccountLoginDlg(CTankLoginPlusDlg* mainDlg,CWnd* pParent = nullptr);	// ��׼���캯��

// �Ի�������
#ifdef AFX_DESIGN_TIME
	enum { IDD = IDD_LOGIN };
#endif

protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV ֧��
// ʵ��
protected:
	HICON m_hIcon;

	// ���ɵ���Ϣӳ�亯��
	virtual BOOL OnInitDialog();
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedLogin();

private:
	CTankLoginPlusDlg* mainDlg;
	CString inputID;
	CString inputPassword;
public:
	afx_msg void OnBnClickedRegister();
private:
	BOOL mRememberAccount;
public:
	afx_msg void OnCbnSelchangeMissile();
};

