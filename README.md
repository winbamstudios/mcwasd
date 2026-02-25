# mcwasd
markov chain with a sunny disposition<br>
a basic n-gram language model!!! to use it, just compile it. output from mcwasd --help
```
markov chain with a sunny disposition
usage: mcwasd [file] [temperature value] [amount of n grams]
input files can be either txt files to train on startup or npt files for pretrained
temperature values can vary from 0 to 10, if none is specified 3 is used
n-gram amount is by default 3, or trigram

options:
--help: this command
--pretrain: use it like "mcwasd --pretrain [training data] [output file] [amount of n grams]"
```
training data can be any plain text file. yes, anything. as long as your computer is decently fast the training shouldn't take long
### how to install on linux
```
dotnet build
sudo cp bin/Debug/net8.0 /bin
```
the net8.0 may vary based on version used for compilation

(yes, it's named after the line from the jamiep song, it's the thing that made me make this abomination)
