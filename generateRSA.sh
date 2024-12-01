#!/bin/bash

ssh-keygen -t rsa -b 2048 -m PEM -f key
openssl rsa -in key -pubout -outform PEM -out key.pub