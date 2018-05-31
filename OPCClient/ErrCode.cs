using System;

namespace OPCClient
{
	internal enum ErrCode
	{
		ok,
		initFileNotFound,
		initFileIncorrect,
		serverNoAccess,
		serverConnFlt,
		itemReadNoCorrect,
		itemWriteNoCorrect,
		nameNotFound,
		addrNotFound,
		varTypeOther,
		sameNameOrAddr,
		noNameOrAddr,
		xmlNoElement,
		xmlNoNode,
		xmlTypeRWFlt
	}
}
