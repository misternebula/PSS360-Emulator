# HD6301Y0 / PSS-360 Emulation

Basic emulator for running a HD6301Y0 ROM, together with an OPL2 emulator and port manipulation to emulate a Yamaha PSS-360 synthesizer.

## Dumping a ROM

Massive thanks to [Sean Riddle](https://seanriddle.com/) for providing the information needed to dump the ROM! ([Here](https://www.seanriddle.com/hd6301y0/) is the directory containing the relevant data.)

The basic outline for dumping the ROM involves glitching the CPU into jumping to and executing code from external memory addresses, while still having access to the internal ROM.

## Dependencies
- NAudio
- WoodyOPL
