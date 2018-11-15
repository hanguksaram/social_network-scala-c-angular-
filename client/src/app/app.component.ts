import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'DatingApp-SPA';
  jwtHelper = new JwtHelperService();

  constructor(private authSevice: AuthService) {}

  ngOnInit() {
   
    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user'));
    if (user) {
      this.authSevice.currentUser = user;
      this.authSevice.changeMemberPhoto(user.photoUrl);
    }
    if (token) {
      this.authSevice.decodedToken = this.jwtHelper.decodeToken(token);
    }
  }
}
