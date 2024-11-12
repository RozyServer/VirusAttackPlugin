# Virus Attack Plugin

## Description

The Virus Attack Plugin adds two new commands to SCP: Secret Laboratory:

- `vattack`: Allows players with a CI card to upload a virus to the system via the intercom.
- `dattack`: Allows players with a MTF Captain card to remove the virus from the system.

## Features

- Activation takes 35 seconds (You can change it in the config), displayed with a progress bar broadcast.
- Death, or being cuffed interrupts the process.
- Upon successful upload/removal, a Cassie message is played, and the facility lights turn off and on.

## Installation

1. Place the plugin DLL into your Exiled plugins directory.
2. Configure plugin as needed in your `config-{yourport}.yml` file.
