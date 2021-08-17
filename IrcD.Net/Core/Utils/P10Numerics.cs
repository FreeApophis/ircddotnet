using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcD.Core.Utils
{
    public class P10Numeric
    {

        const int Base = 64;
        static char[] _lookupTable = new char[Base]
          {  'A','B','C','D','E','F','G','H','I','J','K','L','M',
            'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m',
            'n','o','p','q','r','s','t','u','v','w','x','y','z',
            '0','1','2','3','4','5','6','7','8','9','[',']'};

        private readonly int _serverNumeric;
        public int ServerId => _serverNumeric;

        private readonly int? _clientNumeric;
        public int? ClientId => _clientNumeric;

        public bool IsServer => _clientNumeric.HasValue == false;

        private string _numericString;
        private static readonly StringBuilder Builder = new StringBuilder();

        public P10Numeric(int serverNumeric, int? clientNumeric = null)
        {

            _serverNumeric = serverNumeric;
            _clientNumeric = clientNumeric;

            CreateString();
        }

        private void CreateString()
        {
            Builder.Clear();

            NumericString(_serverNumeric, 2);

            if (_clientNumeric.HasValue)
            {
                NumericString(_clientNumeric.Value, 3);
            }

            _numericString = Builder.ToString();

        }

        private void NumericString(int numeric, int length)
        {
            foreach (var index in Enumerable.Range(1, length))
            {
                Builder.Append(_lookupTable[numeric / Power(Base, length - index) % Base]);
            }
        }

        private int Power(int @base, int power)
        {
            return power switch
            {
                0 => 1,
                1 => @base,
                _ => @base * Power(@base, power - 1),
            };
        }

        public override string ToString()
        {
            return _numericString;
        }
    }
}
