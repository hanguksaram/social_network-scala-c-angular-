import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { RolesModalComponent } from '../../roles-modal/roles-modal.component';

import { Helper } from 'src/app/_helpers/helper';



@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[];
  bsModalRef: BsModalRef;
  
  constructor(private adminService: AdminService,
    private modalService: BsModalService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }
  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe(users => {
      console.log(users);
      this.users = users;
    }, error => {
      console.log(error)
    });
  }

  editRolesModal(user: User) {
    const initialState = {
      user,
      roles: this.getRoles(user)
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
    this.bsModalRef.content.updateSelectedRoles.subscribe(values => {
      const rolesToUpdate = {
        roleNames: values.filter(rl => rl.checked).map(rl => rl.name)
      }
      if (rolesToUpdate) {
        this.adminService.updateUserRoles(user, rolesToUpdate).subscribe(() => {
          user.roles = rolesToUpdate.roleNames;
        }, error => console.log(error))
      }
    })
  }
  private getRoles(user: User) {
    const availableRoles = ["Admin", "VIP", "Member", "Moderator"]
   
    //typescript bug: both arrays of the same type Roles[] (where Role is enum) have elements with different value presentation
    //(number, string), so compareArrays method produce bad result despite proper typesafe checking in compile time
    
    //когда у тебя ФП головного мозга D
   
    const comparedRoles = Helper.compareArrays<string>(availableRoles, user.roles, ((x , y) => x === y),
     (x => {
       return {
        name: x,
        value: x,
        checked:false
      }
     }),
     (y => {
      return {
        name: y,
        value: y,
        checked: true
     }
    })) 
    return [...comparedRoles[0], ...comparedRoles[1]]
    
  
  }

}
