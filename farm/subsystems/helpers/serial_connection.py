# Written by Jeffrey Bringolf

import serial


class SerialConnection:
    """ 
    A wrapper around the serial.Serial class which ensures the closing / opening
    of the connection and provides a context manager.
    """

    def __init__(self, serial_name: str) -> None:
        """
        Initializes the serial connection and wraps it with a TextIOWrapper.
        """
        serial_connection = serial.Serial(serial_name, timeout=2)
        serial_connection.reset_input_buffer()
        serial_connection.flush()

        self.connection = serial_connection

    def __enter__(self) -> serial.Serial:
        """
        The enter method called at the beginning of a context management critical section.
        Called at the beginning of a "with x as y:" block.
        See https://www.geeksforgeeks.org/context-manager-in-python/
        """
        return self.connection

    def __exit__(self) -> None:
        """
        The exit method called at the end of a context management critical section.
        Called at the end of a "with x as y:" block
        See https://www.geeksforgeeks.org/context-manager-in-python/
        """
        self.connection.close()

    def __del__(self) -> None:
        """
        A destructor for the SerialConnection class which closes the connection.
        """
        try:
            if not self.connection.closed:
                self.connection.close()
        except AttributeError:
            # Constructor failed to connect, connection was never set.
            pass

    def readline(self) -> str:
        """
        Reads a line of data coming from the serial port.

        Raises
        ------
        serial.SerialException
            Raised when something went wrong reading from the serial connection.
        UnicodeDecodeError
            Raised when the serial line can not be interpreted as utf-8.
        Returns
        -------
        str
            A line read from the serial connection.
        """

        # Try catch for clarity
        try:
            return self.connection.readline().decode('utf-8')
        except UnicodeDecodeError:
            raise
        except serial.SerialException:
            raise
