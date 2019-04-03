// ZtsyLibrary.h

#pragma once

#include <comdef.h>

#pragma region 非托管类相关操作
namespace ZtsyLib
{
	// _variant_t到Object对象
	inline System::Object^ VarToObject(_variant_t var)
	{
		using namespace System::Runtime::InteropServices;
		System::IntPtr^ pvar = gcnew System::IntPtr(&var);
		System::Object^ obj = Marshal::GetObjectForNativeVariant(*pvar);
		return obj;
	}
	// Object对象到_variant_t
	inline _variant_t* ObjectToVar(System::Object^ obj)
	{
		using namespace System::Runtime::InteropServices;
		_variant_t* vt = new _variant_t();
		System::IntPtr^ pvar = (gcnew System::IntPtr((void*)vt));
		Marshal::GetNativeVariantForObject(obj, *pvar);
		return vt;
	}
}
#pragma endregion

#include "../../ZwLib/FrameWork/DataInterface/Siemens/Dave.h"

extern int32_t daveDebugSwitch;
bool DaveFileCheckConnect(DaveFile file);
DaveFile DaveFileOpen(const char * addrIP, uint16_t addrPort);
void DaveFileClose(DaveFile file);

using namespace System;
using namespace System::Runtime::InteropServices;

namespace ZtsyLib {

	public ref class Dave
	{
		DaveFile file;
		DaveDtSrc * DtSrc;
		DaveIntfc * Intfc;

	public:

		Dave()
		{
			// 启动WINDOWS网络环境
			WSADATA wsaData;
			if (0 != WSAStartup(MAKEWORD(0x02, 0x02), &wsaData))
			{
				throw gcnew System::Exception(System::String::Format("Dave Init Error：{0}\n", GetLastError()));
			}

			file = INVALID_SOCKET;
			DtSrc = NULL;
			Intfc = NULL;

		}

		~Dave()
		{
			Stop();
			WSACleanup();
		}

		bool Start(System::String ^ Addr, USHORT Port) // "192.168.2.1" 102
		{
			const char* addr = (const char*)(Marshal::StringToHGlobalAnsi(Addr)).ToPointer();
			// printf("%s\n", addr);
			Stop();
			if (INVALID_SOCKET != (file = DaveFileOpen(addr, Port)))
			{
				DtSrc = DaveNewDtSrc(malloc, file, daveCommTypePG, 1, 0, daveSpeed9300k, 40000000, daveProtoISOTCP);
				if (DtSrc)
				{
					daveDebugSwitch = daveDebugOFF;
					Intfc = DaveNewIntfc(malloc, free, DtSrc);
					if (Intfc)
					{
						if (daveResOK == DaveIntfcCnctPLC(Intfc))
						{
							// DaveIntfcStartPLC(Intfc);
							return true;
						}
						DaveDel(free, Intfc);
					}
					DaveDel(free, DtSrc);
				}
				DaveFileClose(file);
			}
			return false;
		}

		void Stop()
		{
			if (INVALID_SOCKET != file) DaveFileClose(file), file = INVALID_SOCKET;
			if (Intfc != NULL) DaveDel(free, Intfc), Intfc = NULL;
			if (DtSrc != NULL) DaveDel(free, DtSrc), DtSrc = NULL;
		}

		int32_t GetVD(int Pos, float % Result)
		{
			int res = daveResERROR;
			if (Intfc != NULL)
			{
				uint32_t Temp;
				Temp = 0, res = DaveIntfcReadBytes(Intfc, daveDB, 1, Pos, sizeof(Temp), false, &Temp);
				Temp = daveGetFourBytefrom(&Temp);
				float temp;
				memcpy(&temp, &Temp, 4);
				Result = temp;
			}
			return res;
		}

		int32_t SetVD(int Pos, float Result)
		{
			int res = daveResERROR;
			if (Intfc != NULL)
			{
				uint32_t Temp;
				float temp = Result;
				Temp = daveGetFourBytefrom(&temp);
				res = DaveIntfcWriteBytes(Intfc, daveDB, 1, Pos, sizeof(Temp), &Temp);
			}
			return res;
		}

		int32_t GetVW(int Pos, uint16_t % Result)
		{
			int res = daveResERROR;
			if (Intfc != NULL)
			{
				uint16_t Temp;
				Temp = 0, res = DaveIntfcReadBytes(Intfc, daveDB, 1, Pos, sizeof(Temp), false, &Temp);
				Result = daveGetTwoBytefrom(&Temp);
			}
			return res;
		}

		int32_t SetVW(int Pos, uint16_t Result)
		{
			int res = daveResERROR;
			if (Intfc != NULL)
			{
				uint16_t Temp;
				Temp = daveGetTwoBytefrom(&Result);
				res = DaveIntfcWriteBytes(Intfc, daveDB, 1, Pos, sizeof(Temp), &Temp);
			}
			return res;
		}

		int32_t GetMB(int Pos, uint8_t % Result)  //
		{
			int res = daveResERROR;
			if (Intfc != NULL)
			{
				uint8_t Temp;
				Temp = 0, res = DaveIntfcReadBytes(Intfc, daveFlags, 0, Pos, sizeof(Temp), false, &Temp);
				Result = Temp;
			}
			return res;
		}

		int32_t SetMB(int Pos, uint8_t Result) ///
		{
			int res = daveResERROR;
			if (Intfc != NULL)
			{
				uint8_t Temp;
				Temp = Result;
				res = DaveIntfcWriteBytes(Intfc, daveFlags, 0, Pos, sizeof(Temp), &Temp);
			}
			return res;
		}

		int32_t GetIO(int Pos, bool % Result)
		{
			int res = daveResERROR;
			if (Intfc != NULL)
			{
				uint8_t Temp;
				Temp = 0, res = DaveIntfcReadBytes(Intfc, daveInputs, 0, Pos, sizeof(Temp), true, &Temp);
				Result = Temp;
			}
			return res;
		}

		int32_t GetQO(int Pos, bool % Result)
		{
			int res = daveResERROR;
			if (Intfc != NULL)
			{
				uint8_t Temp;
				Temp = 0, res = DaveIntfcReadBytes(Intfc, daveOutputs, 0, Pos, sizeof(Temp), true, &Temp);
				Result = Temp;
			}
			return res;
		}

		int32_t GetPosT(int Pos, bool % Result)
		{
			int res = daveResERROR;
			if (Intfc != NULL)
			{
				uint8_t Temp[5] = { 0 };
				res = DaveIntfcReadBytes(Intfc, daveTimer200, 0, Pos, 1, false, &Temp);
				Result = Temp[0];
			}
			return res;
		}
	};
}
