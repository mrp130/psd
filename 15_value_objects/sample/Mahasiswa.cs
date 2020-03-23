
using System;

namespace ValueObject.Sample
{
    public class Mahasiswa
    {
        private NIM NIM;
        private string name;
        private DateTime tanggalLahir;

        public Mahasiswa(string NIM, string name, DateTime tanggalLahir)
        {
            this.NIM = new NIM(NIM);
            this.name = name;
            this.tanggalLahir = tanggalLahir;
        }

        public override bool Equals(object obj)
        {
            var m = obj as Mahasiswa;
            if (m == null) return false;

            return m.NIM.Equals(this.NIM);
        }
    }

    public class NIM
    {
        private string value;

        public NIM(string value)
        {
            if (value.Length != 10)
                throw new Exception("nim must be 10 char");

            this.value = value;
        }

        public override bool Equals(object obj)
        {
            var n = obj as NIM;
            if (n == null) return false;

            return n.value == this.value;
        }
    }
}
