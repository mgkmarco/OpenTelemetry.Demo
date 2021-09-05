# https://www.rabbitmq.com/passwords.html
#!/bin/bash
# example:>$ ./passhash.sh "this is the password that i want to hash"
PWD_HEX=$(echo -n $1 | xxd -p)
SALT="908D C60A" 
HEX="$SALT $PWD_HEX"
SHA256=$(echo -n $HEX | xxd -r -p | sha256sum)
# This is thw pwd to be inserted on your rabbit load_definitions file
echo "908D C60A $SHA256" | xxd -r -p | base64 
