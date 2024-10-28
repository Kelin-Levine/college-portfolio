package frc.robot.commands.ScoreCommands;

import edu.wpi.first.wpilibj2.command.Command;
import frc.robot.subsystems.BlinkinSubsystem;
import frc.robot.subsystems.SmartSwerveDriveSubsystem;

public class AutoFaceCommand extends Command {

   
    private final SmartSwerveDriveSubsystem swerveDriveSubsystem;
    private boolean atTarget;
    
    public AutoFaceCommand(SmartSwerveDriveSubsystem swerveDriveSubsystem){
        this.swerveDriveSubsystem = swerveDriveSubsystem;
        this.atTarget = false;
        //addRequirements(swerveDriveSubsystem);
    }


    @Override 
    public void execute(){
        swerveDriveSubsystem.enableAutoFace(true);
        atTarget = swerveDriveSubsystem.alignedToGoal();
    }

    @Override
    public void end(boolean interrupted){

        swerveDriveSubsystem.enableAutoFace(false);
        atTarget = false;
        BlinkinSubsystem.green();
    }

    public boolean isAtTarget() {
        return atTarget;
    }

}

