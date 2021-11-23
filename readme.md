# Passwordles auth app

A basic application which tries to implement an email only login, enabling users to confirm the login on any device, without any magic links, which requires you to confirm your login in the same browser.

## Flow

1. Login with mail (Loading on site appears with instructions)
2. Open mail and click confirm (you can open the mail on a different device and confirm in that device too)
3. Go back to the opened page where the Loading was (you are magically logged in)

### Rest implementation

Relies on checking the confirmation status in interwals

status: In progress

### Realtime implementation (SignalR)

The server automatically handles the confirmation response to the client

status: Todo

