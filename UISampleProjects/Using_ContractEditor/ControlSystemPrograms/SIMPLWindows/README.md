# CH5 Sample Control System Programs - SIMPL

Sample SIMPL project to interface with the sample UI projects

# See Crestron Labs for documentation

http://www.crestronlabs.com/forumdisplay.php?229-Crestron-HTML5-Lab

# Set up touch screen(s) and control system

Using Crestron Toolbox set up the IP Tables of the touch screen(s) and control system

Using Crestron Toolbox, note the the IP ID of the touch screen(s) 

# Open SIMPL Program and verify reference to the UI Contract / Update contract data on contract changes

Install and launch SIMPL (version 4.14.08 or higher)

Navigate to and open .\ContrlSystemPrograms\SIMPLWindows\CH5-sample.smw

In the Program Tree, expand the Ethernet card and review the GUI Extenders associated with each touch screen

Select Alt+U to display the GUI Extender Management dialog and review the the .CHD file assigned to each touch screen
If updates are made to the contract, rebuild the contract in Contract Editor and:
		- 	copy the .\ContractEditor\Ch5SampleContract\output\Ch5_Sample_Contract\SIMPL\Ch5_Sample_Contract.chd to
			         .\ControlSystemPrograms\CHD\Ch5_Sample_Contract.chd
		- In SIMPL, Select Alt+U to display the GUI Extender Management dialog
			- select all touch screens
			- select Sync
			- select Commit changes
    
# Update the Control System Model and Touch Screen addresses

The sample program targets an MC3.  If another control system is being used, from the System Configuration tree in SIMPL:
     - review the GUI Extenders under the touch screens 
     - right click on the MC3
     - select Replace Device
     - select the appropriate 3-series control system 
     - save the program 
     - compile the program and upload to the control system

Update the IP ID(s) of the touch screen in the program, to match that of the IP ID of the physical touch screen(s) being used.

# Upload the program to the control system
 
In SIMPL, compile and upload the CH5-CE-sample.smw program to the control system

# Initializing the data on the touch screen

When the SIMPL program has been uploaded to the control system, press the Power button on the touch screen(s) to initialize the data for each component.




