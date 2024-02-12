
export const rootUrl = 'http://localhost:5070'
export const userController = rootUrl + '/User'
export const CURRENT_USER = "currentUser"
export const videoController = rootUrl + '/Video'
export const profilePicture = (userId) => userController + '/GetUserPfp/' + userId
export const thumbnail = (thumbnailId) => videoController + '/GetThumbnail/' + thumbnailId
export const thumbnailByVideoId = (videoId) => videoController + '/GetThumbnailByVideoId/' + videoId
export const videoContent = (contentId)=>videoController+`/GetVideoContent/`+contentId
export const mod = (n,m) => {
    return ((n % m) + m) % m;
}
export const formatRelativeTime= (date)=> {
    const now = new Date();
    const diff = now - date;
  
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    const months = Math.floor(days / 30);
    const years = Math.floor(days / 365);
  
    if (years > 0) {
      return years === 1 ? "a year ago" : `${years} years ago`;
    } else if (months > 0) {
      return months === 1 ? "a month ago" : `${months} months ago`;
    } else if (days > 0) {
      return days === 1 ? "a day ago" : `${days} days ago`;
    } else if (hours > 0) {
      return hours === 1 ? "an hour ago" : `${hours} hours ago`;
    } else if (minutes > 0) {
      return minutes === 1 ? "a minute ago" : `${minutes} minutes ago`;
    } else {
      return "just now";
    }
}
  
export function parseAndFormatDate(isoDateString) {
  const date = new Date(isoDateString);
  const options = { year: 'numeric', month: '2-digit', day: '2-digit' };

  return date.toLocaleDateString('en-US', options).replace(/\//g, '-');
}

