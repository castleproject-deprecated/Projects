/*=======================================================================
  Copyright (C) Microsoft Corporation.  All rights reserved.
 
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
=======================================================================*/

namespace Cassini {
    using System;
    using System.Collections;
    using System.Text;

    internal class ByteParser {
        private byte[] _bytes;
        private int _pos;

        internal ByteParser(byte[] bytes) {
            _bytes = bytes;
            _pos = 0;
        }

        internal int CurrentOffset
        {
            get { 
                return _pos; 
            }
        }

        internal ByteString ReadLine()
        {
            ByteString line = null;

            for (int i = _pos; i < _bytes.Length; i++) {
                if (_bytes[i] == (byte)'\n') {
                    int len = i-_pos;
                    if (len > 0 && _bytes[i-1] == (byte)'\r') {
                        len--;
                    }

                    line = new ByteString(_bytes, _pos, len);
                    _pos = i+1;
                    return line;
                }
            }

            if (_pos < _bytes.Length) {
                line = new ByteString(_bytes, _pos, _bytes.Length-_pos);
            }

            _pos = _bytes.Length;
            return line;
        }
    }
}
