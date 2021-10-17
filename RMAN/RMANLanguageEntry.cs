namespace RMAN_Parse.RMAN
{
    class RMANLanguageEntry
    {
        public int Offset { get; set; }
        public int TableoffsetOffset { get; set; }
        public int Id { get; set; }
        public int NameOffset { get; set; }
        public string Name { get; set; }
    }
}
