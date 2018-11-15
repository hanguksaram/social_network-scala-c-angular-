import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { BehaviorSubject } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
baseUrl = environment.apiUrl + 'auth/';
jwtHelper = new JwtHelperService();
decodedToken: any;
currentUser: User;
photoUrl = new BehaviorSubject<string>('../../assets/user.png');
currentPhotoUrl = this.photoUrl.asObservable();

constructor(private http: HttpClient) { }

changeMemberPhoto(photoUrl: string) {
  this.photoUrl.next(photoUrl);
  this.currentUser.photoUrl = photoUrl;
  localStorage.setItem('user', JSON.stringify(this.currentUser))

  ;
}

login(model: any) {
  return this.http.post(this.baseUrl + 'login', model)
    .pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          const { user: usr } = user;
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(usr));
          this.currentUser = usr;
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
}
login2(model: any) {
  this.http.post(this.baseUrl + 'login', model).subscribe(
    response => {
      const user = response;
      if (user) {
        console.log(user);
      }
    }
  );
}

register(user: User) {
  return this.http.post(this.baseUrl + 'register', user);
}
loggedIn() {
  const token = localStorage.getItem('token');
  return !this.jwtHelper.isTokenExpired(token);
}

roleMatch(allowedRoles): boolean {
  
  const userRoles = this.decodedToken.role as Array<String>;
  const isMatch = allowedRoles.some(role => userRoles.includes(role))
  return isMatch;
}
}

