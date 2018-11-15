import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
  @ViewChild('memberTabs') memberTabs: TabsetComponent;
  user: User;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    // loading data using route resolver
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    //passing queryparams through router
    this.route.queryParams.subscribe(params => {
      const selectedTab = params['tab'];
      if (selectedTab > 0) {
        this.selectTab(selectedTab);
      }

    })

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ];
    this.galleryImages = this.getImages();

  }
  getImages() {
    return this.user.photos.map(img =>
      ({small: img.url,
        medium: img.url,
        big: img.url,
        description: img.description}));
  }
  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  // loadUser() {
  //   this.userService.getUser(+this.route.snapshot.params['id']) // way to get param frm url
  //     .subscribe(user => this.user = user,
  //       error => this.alertify.error(error)); // + before string in param - its trick to convert to number
  // }

}
