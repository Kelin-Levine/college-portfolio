package frc.robot.commands.ScoreCommands;

import edu.wpi.first.wpilibj.GenericHID.RumbleType;
import edu.wpi.first.wpilibj.XboxController;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;
import edu.wpi.first.wpilibj2.command.Command;

public class AutoTargetNotifyCommand extends Command {

   
    //private final XboxController controller;
    
    public AutoTargetNotifyCommand(/* XboxController controller */){
        SmartDashboard.putBoolean("READY TO FIRE", false);
        //this.controller = controller;
    }


    @Override 
    public void execute(){
        SmartDashboard.putBoolean("READY TO FIRE", true);
        //controller.setRumble(RumbleType.kBothRumble, 1);
    }

    @Override
    public void end(boolean interrupted){
        SmartDashboard.putBoolean("READY TO FIRE", false);
        //controller.setRumble(RumbleType.kBothRumble, 0);
    }

}

