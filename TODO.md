# TODO

- [x] Use DB connection pooling (for free in MySQL connector)
- [x] Hierarchial config files
- [x] Get rid of the `Query<dynamic>` stuff and use type mapping / FluentMapper
- [x] Figure out multi-species support (just going to index everything by species database name, eg `homo_sapiens_core_99_38`)
- [ ] Probably need to redo the `Slice` API since that means something totally different in `Bio::EnsEMBL`
- [ ] ...or just add a `Root` object to enumerate species and get access to slices
- [ ] Make a nuget package
- [ ] Make the cache much smarter about DNA ranges (eg. don't load the entire sequence from the database, but only the parts asked for)
- [ ] For antisense sequences (orientation == -1, aka 3'-to-5'), cache them reverse-complemented already so we're not doing that every time
