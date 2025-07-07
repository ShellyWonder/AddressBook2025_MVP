//toastMesssage.js

export function initToast(toastEl) {
    console.log(">>> initToast called!", toastEl);
    const toast = bootstrap.Toast.getOrCreateInstance(toastEl);
    toast.show();
}