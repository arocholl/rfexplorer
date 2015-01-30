//============================================================================
//RF Explorer for Windows - A Handheld Spectrum Analyzer for everyone!
//Copyright © 2010-15 Ariel Rocholl, www.rf-explorer.com
//
//This application is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 3.0 of the License, or (at your option) any later version.
//
//This software is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//General Public License for more details.
//
//You should have received a copy of the GNU General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RFExplorerCommunicator
{

    public enum eExtFlashDataType
    {
        //Some Reserved values for future data sweep versions and to make easier use ranges for < or >
        FLASH_DATA_TYPE_SWEEP_v1    =0,                
        FLASH_DATA_TYPE_SWEEP_START =15,        
        FLASH_DATA_TYPE_CONFIG      =20,
        FLASH_DATA_TYPE_BITMAP      =100,
        FLASH_DATA_TYPE_API,
    
        FLASH_DATA_TYPE_EMPTY       =0xff
    };   

    public struct stFLASH_SweepStart
    {
        eExtFlashDataType   m_eDataType;
        byte                m_nSizeBlock16;
        byte                m_nRecordID;
        byte                m_nInterval;
        char[]              m_sLabel; //note this comes from C and therefore last char must be zero
        //UINT16              nTotalScreens; //we do not save this one as will burn the FLASH before knowing final value

        public stFLASH_SweepStart(ref byte[] arrDataBlock, int nOffset)
        {
            m_eDataType = eExtFlashDataType.FLASH_DATA_TYPE_SWEEP_START;
            if (nOffset < arrDataBlock.Length - 3)
            {
                m_nSizeBlock16 = arrDataBlock[nOffset + 1];
                m_nRecordID = arrDataBlock[nOffset + 2];
                m_nInterval = arrDataBlock[nOffset + 3];
            }
            else
            {
                m_nSizeBlock16 = 0;
                m_nRecordID = 0;
                m_nInterval = 0;
            }
            m_sLabel = new char[9];
            for (int nInd = 0; nInd < 9; nInd++)
                m_sLabel[nInd] = (char)arrDataBlock[nOffset + 4 + nInd];
        }

        public int SizeBytes
        {
            get { return m_nSizeBlock16 * 16; }
        }

        public override string ToString()
        {
            return "Data stFLASH_SweepStart - Size:" + m_nSizeBlock16 * 16 + " bytes, ID:" + m_nRecordID + ", Interval:" + m_nInterval + ", Label:" + m_sLabel;
        }
    };

    public struct stFLASH_SweepData_v1
    {
        eExtFlashDataType   m_eDataType;        //Data type FLASH_DATA_TYPE_SWEEP_v1
        byte m_nSizeBlock16;     //Size in 16 bytes blocks, e.g. a 4=16*4=64 bytes total.
        UInt32              m_nFreqStartKHZ;    //Start frequency of the sample
        UInt32              m_nFreqStepHZ;      //Step frequency of the sample
        byte                m_nSweepPoints;     //Number of points per sweep
        RFECommunicator.eMode m_eMode;          //Mode when this was recorded
        byte[]              m_arrSweepPoints;   //Array with all sweep points downloaded

        public stFLASH_SweepData_v1(ref byte[] arrDataBlock, int nOffset)
        {
            m_eDataType = eExtFlashDataType.FLASH_DATA_TYPE_SWEEP_v1;
            if (nOffset < arrDataBlock.Length - 1)
                m_nSizeBlock16 = arrDataBlock[nOffset + 1];
            else
                m_nSizeBlock16 = 0;
            m_nFreqStartKHZ = (UInt32)(arrDataBlock[nOffset + 2] + arrDataBlock[nOffset + 3] * 0x100 + arrDataBlock[nOffset + 4] * 0x10000 + arrDataBlock[nOffset + 5]*0x1000000);
            m_nFreqStepHZ = (UInt32)(arrDataBlock[nOffset + 6] + arrDataBlock[nOffset + 7] * 0x100 + arrDataBlock[nOffset + 8] * 0x10000 + arrDataBlock[nOffset + 9] * 0x1000000);
            m_nSweepPoints = arrDataBlock[nOffset + 10];
            m_eMode = (RFECommunicator.eMode)arrDataBlock[nOffset + 11];
            m_arrSweepPoints = new byte[m_nSweepPoints];
            for (int nInd = 0; nInd < m_nSweepPoints; nInd++)
            {
                m_arrSweepPoints[nInd] = arrDataBlock[nOffset + 12 + nInd];
            }
        }

        public int SizeBytes
        {
            get { return m_nSizeBlock16 * 16; }
        }

        public override string ToString()
        {
            return "Data stFLASH_SweepData_v1 - Size:" + m_nSizeBlock16 * 16 + " bytes, Start:"
                + m_nFreqStartKHZ + "KHz, Step:" + m_nFreqStepHZ + "Hz, Total points:" + m_nSweepPoints + ", Mode:" + m_eMode;
        }

    };

    /// <summary>
    /// This class represents a basic block of memory, up to 4096 bytes length, with an address within the available memory space, 
    /// a total length and a raw memory container
    /// </summary>
    public class RFEMemoryBlock
    {
        public const UInt16 MAX_BLOCK_SIZE = 4096;

        /// <summary>
        /// Memory container, values out of range are initialized to 0xFF
        /// </summary>
        byte[] m_arrBytes = new byte[MAX_BLOCK_SIZE];
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] DataBytes
        {
            get { return m_arrBytes; }
        }
        
        /// <summary>
        /// Valid address within the memory space this object is defined. For instance the external FLASH has a range of 2MB
        /// </summary>
        UInt32 m_nAddress = 0;
        public UInt32 Address
        {
            get { return m_nAddress; }
            set { m_nAddress = value; }
        }

        /// <summary>
        /// Size of the block in bytes, being MAX_BLOCK_SIZE the maximum value
        /// </summary>
        UInt16 m_nSize = 0;
        public UInt16 Size
        {
            get { return m_nSize; }
            set { m_nSize = value; }
        }

        public RFEMemoryBlock()
        {
            for (int nInd = 0; nInd < MAX_BLOCK_SIZE; nInd++)
            {
                m_arrBytes[nInd] = 0xff; //initialize with same values to imitate internal memory status
            }
        }
    }
}
