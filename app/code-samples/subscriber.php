<?php
/**
 * This file contains a class for representing subscribers to the directory. It 
 * is also used in the Acl as the Users class.
 *
 * PHP Version 5
 *
 * @package     Directory
 * @subpackage  Models
 * @author      Randall Loffelmacher <randall@loffelmacher.com>
 */
 
 /**
 * This model class represents a subscriber, which are the users in the Acl.
 * 
 */
class Subscriber extends AppModel 
{
    /**
    * Helps netbeans do auto-completion
    *
    * @var $this Model 
    */
    public $name = 'Subscriber';
    public $displayField = 'name';
    public $actsAs = array(
                    'Acl' 	=> 'requester',
                    'Containable');
    public $virtualFields = array(
        'name'	=> 'CONCAT(Subscriber.first_name,\' \',Subscriber.last_name)');
    public $contain = array('Group');
        
    /**
     * This is part of the Acl system, a requester.
     *
     * @return array Returns the parent node (usually a Group) or null if no parent.
     */
    public function parentNode() {
        if (!$this->id && empty($this->data)) {
            return null;
        }
        $data = $this->data;
        if (empty($this->data)) {
            $data = $this->read();
        }
        if (!$data['Subscriber']['group_id']) {
            return null;
        } else {
            return array('Group' => array('id' => $data['Subscriber']['group_id']));
        }
    }


    /**
     * This is used in validating password changes, ensures the password confirmation
     * matches the password.
     *
     * @return boolean True if the password & confirmation match, false otherwise.
     */
    public function confirmPassword() {
        if ($this->data['Subscriber']['password']
            == Security::hash(
            Configure::read('Security.salt').$this->data['Subscriber']['password_confirm'])
        ) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * This is used when changing the password, we request current password before 
     * changing the password.
     *
     * @param array An array containing the users old password.
     * 
     * @return boolean Returns true if the user provides correct current password, 
     *                  false otherwise.
     */
    public function checkCurrentPassword($check) {
        $thisSub = $this->findAllById($this->data['Subscriber']['id']);
        if ($thisSub[0]['Subscriber']['password'] ==
            Security::hash(Configure::read('Security.salt').$check['old_password'])
        ) {
            return true;
        } else {
            return false;
        }
    }
    
    /**
     * This routine creates instances of Subscriber class, 
     * and is called by the signup() action of the Subscribers controller
     *
     * @param mixed $data The form data for user signup.
     *
     * @return mixed Returns the new Subscriber on success, null otherwise.
     */
    public function initNewSubscriber($data) {
        $groups = ClassRegistry::init('Group');
        $data['Subscriber']['group_id'] = 
                        $groups->field('id', array('name =' => 'Subscribers'));
        $this->create($data);
        if ($this->save()) {
            return $this;
        } else {
            return null;
        }
    }

    /**
     * This is called to update Subscriber data.
     *
     * @param int $id the subscribers.id field
     * @param object $data the form data
     *
     * @return integer Zero indicates success, non-zero indicates failure.
     */
    public function update($id, $data) {
        if (empty($data['Subscriber']['old_password'])) {
            //dont update the password if they aren't passing in the old password
            if ($this->save(
                $data, true, array('first_name', 'last_name', 'email', 'username'))
            ) {
                return 0; // success
            } else {
                return 1; // failure
            }
        } else {
            if ($this->save($data, true, array(
                        'first_name', 'last_name', 'email', 
                        'username', 'password', 'password_confirm', 
                        'old_password'))
            ) {
                return 0; // success
            } else {
                return 2; // failure
            }
        }
        return 3; // failure
    }
    
    /**
     * Used to determine if a user has a current active subscription. This is 
     * called from directories_controller.php to allow/deny access to the 
     * directory
     *
     * @param integer $sub_id The id of the Subscriber
     *
     * @return boolean True if subscriber is allowed, false otherwise.
     */
    public function isSubscribed($sub_id) {
        $temp = $this->Subscription->find('all', array(
            'conditions' => array(
                'paid' => 1,
                'subscriber_id' => $sub_id,
                'CURDATE() BETWEEN DATE(starts) AND DATE(ends)'
            )
        ));
        if (!empty($temp)) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * This is used to allow Subscribers to reset their password if they forget.
     *
     * @param object $data The form data.
     * @param object $otp The Otp component which performs password reset logic.
     *
     * @return array index of 0 => message, index of 1 => [link,email]
     */
    public function requestPwdReset($data, $otp) {
        if ($data != null && isset($data) && isset($data['email'])) {
            $sub = $this->findByEmail($data['email'], array('contain'=>array()));
            if ($sub!=null) { 
                $now = microtime(true);
                $ttl =$now + 48*3600; // the invitation is good for the next two days
                $otp = $otp->createOTP(
                    array (
                        'user'=>$sub['Subscriber']['email'],
                        'password'=>$sub['Subscriber']['password'],
                        'ttl'=> $ttl
                    )
                );
                $link = '<a href="'.BASE_URL."subscribers/reset_password/".
                        $sub['Subscriber']['email']."/".$ttl."/".$otp.
                        '">Reset Your Password</a>';
                return array(1=>array(
                    'link'	=> $link,
                    'Subscriber' => $sub['Subscriber'],
                ));
            } else {
                return array(0=>array(
                    'message' => 'That email was not found. Please try again.',
                ));
            }
        }
        return null;
    }
    
    /**
     * Performs an actual password change if the Otp components validates the request 
     *
     * @param mixed $data the form data (password & password confirmation)
     * @param string $email the email address of subscriber
     * @param string $ttl the amount of time the request is valid
     * @param string $otp_string the secret
     * @param mixed $otp the Otp component passed in by the controller
     * @param mixed $auth the Auth component passed in by the controller
     *
     * @return mixed Returns 0 for success, 1 => array() for failure.
     */
    public function resetPwd($data, $email, $ttl, $otp_string, $otp, $auth) {
        $user = $this->findByEmail($email);
        if($user){
            $passwordHash = $user["Subscriber"]["password"];
            $now = microtime(true);
            // check expiration date. the experation date should be greater them now.
            if($now <  $ttl){
                // validate OTP
                if($otp->authenticateOTP($otp_string,array('user'=>$email, 'password'=>$passwordHash, 'ttl'=> $ttl))){
                    // set the password
                    $this->id =  $user["Subscriber"]["id"];
                    $groups = ClassRegistry::init('Group');
                    $this->set(array(
                        'password' => $auth->password($data["Subscriber"]["password"]),
                        'password_confirm' => $data["Subscriber"]['password_confirm'],
                        'group_id' => $groups->field('id', array('name =' => 'Subscribers')),
                    ));
                    if ($this->save(null, array(
                            'validate' => true,
                            'fieldList' => array('password', 'password_confirm'),
                        ))
                    ) {
                        return 0;
                    } else {
                        return array(1=>array()); 
                    }
                } else {
                    //redirects to /
                    return array(1=>array(
                        'message' 	=> "Invalid request. Please contact the website administration."
                    ));
                }
            } else {
                //redirects to /
                return array(1=>array(
                    'message' => "Your invitation has expired. Please contact the website administration.",
                ));
            }
        }
        return null;
    }
    
    /**
     * This is the validation for subscribers. Refers to validation methods in 
     * app_model.php for first & last name and phone number.
     *
     */
    public $validate = array(
        'first_name' => array(
            'alphanumericwithspaces' => array(
                'rule' => array('nameValid', 'first_name'),
                'message' => 'Only letters and numbers are allowed',
            ),
            'notempty' => array(
                'rule' => array('notempty'),
                'message' => 'First name cannot be empty',
            ),
        ),
        'last_name' => array(
            'alphanumericwithspaces' => array(
                'rule' => array('nameValid', 'last_name'),
                'message' => 'Only letters and numbers are allowed',
            ),
            'notempty' => array(
                'rule' => array('notempty'),
                'message' => 'Last name cannot be empty',
            ),
        ),
        'email' => array(
            'email' => array(
                'rule' => array('email'),
                'message' => 'Must be a valid email',
            ),
            'notempty' => array(
                'rule' => array('notempty'),
                'message' => 'Email cannot be empty',
            ),
            'isUnique' => array(
                'rule' => array('isUnique'),
                'message' => 'This email address is already registered',
            )
        ),
        'username' => array(
            'alphanumeric' => array(
                'rule' => array('custom', '/^[a-z0-9@\.]*$/i'),
                'message' => 'Only letters and numbers are allowed',
            ),
            'notempty' => array(
                'rule' => array('notempty'),
                'message' => 'Username cannot be empty',
            ),
            'isUnique' => array(
                'rule' => array('isUnique'),
                'message' => 'This username is already registered',
            )
        ),
        'password_confirm' => array(
            'notempty' => array(
                'rule' => array('notempty'),
            ),
            'minLength' => array(
                'rule' => array('minLength', 8),
                'message' => 'Password must be at least 8 characters long'
            ),
            'match' => array(
                'rule' => array('confirmPassword', 'password'),
                'message' => 'Passwords do not match',
            ),
        ),
        'old_password' => array(
            'match' => array(
                'rule' => array('checkCurrentPassword'),
                'message' => 'Current password was incorrect',
            )
        ),
        'group_id' => array(
            'numeric' => array(
                'rule' => array('numeric'),
            ),
        ),
    );

    /**
     * The belongsTo relationships for Subscriber.
     *
     */
    public $belongsTo = array(
        'Group' => array(
            'className' => 'Group',
            'foreignKey' => 'group_id',
            'conditions' => '',
            'fields' => '',
            'order' => ''
        )
    );

    /**
     * The hasMany relationships for Subscriber. Only Subscription for now.
     *
     */
    public $hasMany = array(
        'Subscription' => array(
            'className' => 'Subscription',
            'foreignKey' => 'subscriber_id',
            'dependent' => true,
            'conditions' => '',
            'fields' => '',
            'order' => '',
            'limit' => '',
            'offset' => '',
            'exclusive' => '',
            'finderQuery' => '',
            'counterQuery' => ''
        )
    );
}
