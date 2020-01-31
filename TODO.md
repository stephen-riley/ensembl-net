# TODO

- [ ] Use DB connection pooling
- [ ] Get rid of the `Query<dynamic>` stuff and use type mapping / FluentMapper
- [ ] Probably need to redo the `Slice` API since that means something totally different in `Bio::EnsEMBL`
- [ ] Figure out multi-species support (is that multiple databases? multiple schemas? surely Ensembl has a way to do this...)
- [ ] Make the cache much smarter about DNA ranges (eg. don't load the entire sequence from the database, but only the parts asked for)
- [ ] For antisense sequences (orientation == -1, aka 3'-to-5'), cache them reverse-complemented already so we're not doing that every time
